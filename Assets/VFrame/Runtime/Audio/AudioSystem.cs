using System.Collections.Generic;
using VContainer.Unity;

namespace Audio
{
    public class AudioSystem : IInitializable
    {
        private static AudioSystem _sharedInstance;

        // private readonly Dictionary<>

        public AudioSystem(AudioGroup[] groups)
        {
            _sharedInstance = this;
            foreach (var audioGroup in groups)
            {
                foreach (var reference in audioGroup.Clips)
                {
                }
            }
        }

        public void Initialize()
        {
        }
    }
}