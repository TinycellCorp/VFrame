using Cysharp.Threading.Tasks;

namespace VFrame.Scene
{
    public interface IPreloader
    {
        UniTask PreloadAsync(AssetCache cache);
    }

    public class NothingPreload : IPreloader
    {
        public UniTask PreloadAsync(AssetCache cache)
        {
            return UniTask.CompletedTask;
        }
    }
}