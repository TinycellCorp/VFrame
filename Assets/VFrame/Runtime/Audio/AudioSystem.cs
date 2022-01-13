using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using VFrame.Extension;
using Object = UnityEngine.Object;

namespace Audio
{
    public interface IAudioSourcePlayer
    {
        void Ready(AudioSource source);
        void Play(AudioSource source, AudioClip clip);
    }

    public class PlayOneShotPlayer : IAudioSourcePlayer
    {
        public void Ready(AudioSource source)
        {
            source.loop = false;
        }

        public void Play(AudioSource source, AudioClip clip)
        {
            source.PlayOneShot(clip);
        }
    }

    public class BGMPlayer : IAudioSourcePlayer
    {
        public void Ready(AudioSource source)
        {
            source.loop = true;
        }

        public void Play(AudioSource source, AudioClip clip)
        {
            source.Stop();
            source.clip = clip;
            source.Play();
        }
    }

    public class AudioSystem : IInitializable
    {
        private static AudioSystem _sharedInstance;

        private readonly Dictionary<string, AudioSource> _mixerNameToAudioSources =
            new Dictionary<string, AudioSource>();

        private readonly Dictionary<string, (AudioGroup group, int clipIndex)> _keyToAudioData =
            new Dictionary<string, (AudioGroup group, int clipIndex)>();

        private readonly Dictionary<string, AudioClipReference> _keyToClipReferences =
            new Dictionary<string, AudioClipReference>();

        private readonly Dictionary<string, IAudioSourcePlayer> _sourcePlayers;

        private readonly GameObject _audioSourceParent;

        public AudioSystem(AudioGroup[] groups, Dictionary<string, IAudioSourcePlayer> players)
        {
            _sharedInstance = this;
            _sourcePlayers = players ?? new Dictionary<string, IAudioSourcePlayer>();

            _audioSourceParent = new GameObject("AudioSystem");
            Object.DontDestroyOnLoad(_audioSourceParent);

            foreach (var audioGroup in groups)
            {
                MergeGroup(audioGroup, _audioSourceParent.transform);
            }
        }

        public void Initialize()
        {
        }

        private void MergeGroup(AudioGroup audioGroup, Transform parent)
        {
            var mixerName = audioGroup.MixerGroup.name;
            if (!_mixerNameToAudioSources.ContainsKey(mixerName))
            {
                var source = new GameObject(mixerName).AddComponent<AudioSource>();
                source.transform.SetParent(parent);
                source.outputAudioMixerGroup = audioGroup.MixerGroup;
                source.playOnAwake = false;

                if (_sourcePlayers.TryGetValue(mixerName, out var player))
                    player.Ready(source);

                _mixerNameToAudioSources.Add(mixerName, source);
            }

            for (int i = 0; i < audioGroup.Clips.Length; i++)
            {
                var clip = audioGroup.Clips[i];
                if (!string.IsNullOrEmpty(clip.Key) && clip.Asset.RuntimeKeyIsValid())
                {
                    _keyToAudioData.Add(clip.Key, (audioGroup, i));
                }
#if UNITY_EDITOR
                else
                {
                    if (string.IsNullOrEmpty(clip.Key))
                    {
                        throw new Exception($"EmptyAudioKey: {audioGroup.name} [{i}]");
                    }

                    if (!clip.Asset.RuntimeKeyIsValid())
                    {
                        throw new Exception($"RuntimeKeyIsValid: {audioGroup.name} [{i}]");
                    }
                }
#endif
            }
        }

        private void Play(string mixerName, AudioSource source, AudioClip clip)
        {
            if (_sourcePlayers.TryGetValue(mixerName, out var player))
            {
                player.Play(source, clip);
            }
            else
            {
                source.PlayOneShot(clip);
            }
        }

        private void LazyPlay(string key, string mixerName, AudioSource source, in AudioClipReference reference)
        {
            reference.AsUniTask().ContinueWith(clip => { Play(mixerName, source, clip); });
        }

        private bool IsLoading(string key)
        {
            if (!_keyToClipReferences.TryGetValue(key, out var reference)) return false;

            var status = reference.AsUniTask().Status;
            return status == UniTaskStatus.Pending;
        }

        private async UniTask Preload(string key)
        {
            if (IsLoading(key)) return; //TODO: or ContinueWith
            if (_keyToClipReferences.ContainsKey(key)) return;

            if (!_keyToAudioData.TryGetValue(key, out var data)) return;
            if (!TryLoadAudioClipWithCache(key, data.group, data.clipIndex, out var reference)) return;

            await reference.AsUniTask();
        }

        private async UniTask LoadAudioDataAsync(string key)
        {
            if (_keyToClipReferences.TryGetValue(key, out var reference) && reference.TryGetAsset(out var clip))
            {
                await clip.LoadAudioDataAsync();
            }
            else
            {
                throw new KeyNotFoundException($"not loaded asset! {key}");
            }
        }

        private bool TryLoadAudioClipWithCache(string key, AudioGroup audioGroup, int index,
            out AudioClipReference reference)
        {
            if (index >= audioGroup.Clips.Length)
            {
                reference = null;
                return false;
            }

            reference = audioGroup.Clips[index].Asset.LoadAssetAsync().ToReference();
            _keyToClipReferences.Add(key, reference);
            return true;
        }

        private void LoadAndPlay(string key)
        {
            if (IsLoading(key)) return; //TODO: or ContinueWith
            if (!_keyToAudioData.TryGetValue(key, out var data)) return;
            if (!_mixerNameToAudioSources.TryGetValue(data.group.MixerGroup.name, out var source)) return;

            var mixerName = data.group.MixerGroup.name;
            if (_keyToClipReferences.TryGetValue(key, out var clipReference))
            {
                if (clipReference.TryGetAsset(out var loadedClip))
                {
                    Play(mixerName, source, loadedClip);
                }
                else
                {
                    LazyPlay(key, mixerName, source, clipReference);
                }
            }
            else if (TryLoadAudioClipWithCache(key, data.group, data.clipIndex, out var reference))
            {
                LazyPlay(key, mixerName, source, reference);
            }
        }

        private AudioSourceController AccessAudioSource(string mixerName)
        {
            //TryGetValue(mixerName, source)
            return new AudioSourceController(null);
        }

        public readonly struct AudioSourceController : IDisposable
        {
            private readonly AudioSource _source;

            public float Volume
            {
                get => _source.volume;
                set => _source.volume = value;
            }

            public bool Mute
            {
                get => _source.mute;
                set => _source.mute = value;
            }

            public AudioSourceController(AudioSource source)
            {
                _source = source;
            }

            public void Dispose()
            {
                //TODO: Save Changes
            }
        }

        #region Static Methods

        public static void Play(string key)
        {
            _sharedInstance.LoadAndPlay(key);
        }

        public static async UniTask Use(string key)
        {
            await _sharedInstance.Preload(key);
            await _sharedInstance.LoadAudioDataAsync(key);
        }

        public static void DisposeAudioClip(string key)
        {
            var references = _sharedInstance._keyToClipReferences;
            if (references.TryGetValue(key, out var reference))
            {
                if (reference.TryGetAsset(out var asset))
                {
                    foreach (var source in _sharedInstance._mixerNameToAudioSources.Values)
                    {
                        if (source != null && source.clip == asset)
                        {
                            source.Stop();
                            source.clip = null;
                        }
                    }
                }

                reference.Dispose();
                references.Remove(key);
            }
        }

        #endregion
    }
}