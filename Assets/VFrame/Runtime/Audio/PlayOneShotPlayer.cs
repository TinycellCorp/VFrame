using UnityEngine;

namespace VFrame.Audio
{
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
}