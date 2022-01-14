using UnityEngine;

namespace VFrame.Audio
{
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
}