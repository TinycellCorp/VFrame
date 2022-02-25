using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFrame.UI.Animation;
using VFrame.UI.Command;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.Group;
using VFrame.UI.Pool;
using VFrame.UI.View;
using Object = System.Object;

namespace VFrame.UI.Module.Message
{
    public class MessageGroup<TView> : IGroup where TView : class, IMessageView
    {
        private readonly Config _config;
        private readonly IViewPool<TView> _pool;

        private readonly Queue<IView> _views = new Queue<IView>();
        private CancellationTokenSource _pendingTokenSource;

        private readonly Channel<AsyncLazy> _channel = Channel.CreateSingleConsumerUnbounded<AsyncLazy>();

        public MessageGroup(Config config, IViewPool<TView> pool)
        {
            _config = config;
            _pool = pool;
        }

        public UniTask Push(ISystemContext context, IView view)
        {
            var animation = context.ResolveAnimation(view) as IMessageAnimation;
            if (animation == null)
            {
                throw new NullReferenceException(nameof(IMessageAnimation));
            }

            if (context.View.TryPopManipulator(view, out var manipulator))
            {
                var task = UniTask.Lazy(() => InternalPush(animation, view, manipulator));
                _channel.Writer.TryWrite(task);
            }

            PlayChannel().Forget();
            return UniTask.CompletedTask;
        }

        private UniTaskStatus _playChannelStatus = UniTaskStatus.Succeeded;

        private async UniTaskVoid PlayChannel()
        {
            if (_playChannelStatus == UniTaskStatus.Pending) return;
            _playChannelStatus = UniTaskStatus.Pending;
            while (_channel.Reader.TryRead(out var task))
            {
                await task;
            }

            _playChannelStatus = UniTaskStatus.Succeeded;
        }

        private async UniTask InternalPush(IMessageAnimation animation, IView source, IManipulator manipulator)
        {
            if (_pendingTokenSource != null)
            {
                _pendingTokenSource.Cancel();
                _pendingTokenSource = null;
            }


            if (_views.Count >= _config.Limit)
            {
                var exitView = _views.Dequeue();
                animation.Out(exitView).ContinueWith(() => { _pool.Return(exitView as TView); });
            }

            int index = _views.Count;
            foreach (var v in _views)
            {
                animation.Shift(v, index--);
            }

            var view = _pool.Rent(source);
            view.SetPosition(Vector2.zero);

            await view.Ready();
            await manipulator.Ready(view);
            await animation.In(view);

            _views.Enqueue(view);

            _pendingTokenSource = new CancellationTokenSource();
            UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: _pendingTokenSource.Token).ContinueWith(() =>
            {
                foreach (var v in _views)
                {
                    animation.Hide(v).ContinueWith(() => { _pool.Return(v as TView); });
                }

                _views.Clear();
            });
        }

        public UniTask Pop(ISystemContext context) => UniTask.CompletedTask;

        public void OnImmediatePop(IView view)
        {
        }


        public class Config
        {
            public readonly int Limit;

            public Config(int limit)
            {
                Limit = limit;
            }
        }
    }
}