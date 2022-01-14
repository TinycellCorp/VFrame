using UnityEngine;

namespace VFrame.Audio
{
    public interface IAudioSourcePlayer
    {
        void Ready(AudioSource source);
        void Play(AudioSource source, AudioClip clip);
    }
}