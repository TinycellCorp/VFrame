using Cysharp.Threading.Tasks;
using VFrame.UI.View;

namespace VFrame.UI.Module.Message
{
    public interface IMessageView : IView
    {
    }

    public abstract class MessageView<TView> : ComponentView<TView>, IMessageView where TView : class, IMessageView
    {
        protected override void Awake()
        {
            UISystem.Register<TView, MessageGroup<TView>>(this);
            Init();
        }

        public override UniTask Ready()
        {
            Alpha = 1;
            return UniTask.CompletedTask;
        }
    }
}