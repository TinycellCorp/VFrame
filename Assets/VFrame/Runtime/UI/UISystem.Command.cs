#define DISALBE_COMMAND_LOG

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.Command.Route;
using VFrame.UI.Context;
using VFrame.UI.View;
using UnityEngine;

namespace VFrame.UI
{
    public partial class UISystem : ICommandContext
    {
        private static bool _isBlocking = false;
        private static bool _isIgnoreBlocking = false;
        private static bool _isCommandBlocking = false;

        private static readonly Queue<ICommand> Commands = new Queue<ICommand>();
        private static UniTaskStatus _playCommandStatus = UniTaskStatus.Succeeded;

        private static bool IsBlocking => _isIgnoreBlocking == false && (_isBlocking || _isCommandBlocking);

        private static IView _entryView;

        public ICommandContext Command => this;

        private static bool IsCommandPlayable
        {
            get
            {
                if (_playCommandStatus == UniTaskStatus.Pending) return false;
                if (_systemReadySource.Task.Status == UniTaskStatus.Pending) return false;
                return true;
            }
        }

        private static void PlayCommands()
        {
            if (!IsCommandPlayable) return;

            PlayCommandsAsync().Forget();
        }

        private static async UniTaskVoid PlayCommandsAsync()
        {
            if (!Commands.Any()) return;
            _playCommandStatus = UniTaskStatus.Pending;

            using (EnableCommandBlocking())
            {
                await UniTask.NextFrame(PlayerLoopTiming.PostLateUpdate);

                while (Commands.Any())
                {
                    try
                    {
                        var command = Commands.Dequeue();
#if !DISALBE_COMMAND_LOG && UNITY_EDITOR
                    Debug.Log($"Execute: {command.GetType().Name}");
#endif
                        await command.Execute(_sharedInstance);
                    }
                    catch (Exception e)
                    {
                        Commands.Clear();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        Debug.LogException(e);
#endif
                        break;
                    }
                }
            }

            _playCommandStatus = UniTaskStatus.Succeeded;
        }


        private static void EnqueueCommand(ICommand command)
        {
            Commands.Enqueue(command);
            PlayCommands();
        }

        private static bool SafeEnqueueCommand(ICommand command)
        {
            if (IsBlocking) return false;
            EnqueueCommand(command);
            return true;
        }

        private static void ExecuteCacheCommand(ICommand command)
        {
            if (_container == null)
            {
                EnqueueCommand(command);
            }
            else
            {
                command.Execute(_sharedInstance);
            }
        }


        public static BlockingHandler EnableBlocking()
        {
            _isBlocking = true;
            return new BlockingHandler();
        }

        public readonly struct BlockingHandler : IDisposable
        {
            public void Dispose()
            {
                _isBlocking = false;
            }
        }

        public static IgnoreBlockingHandler IgnoreBlocking()
        {
            _isIgnoreBlocking = true;
            return new IgnoreBlockingHandler();
        }

        public readonly struct IgnoreBlockingHandler : IDisposable
        {
            public void Dispose()
            {
                _isIgnoreBlocking = false;
            }
        }
        
        public static CommandBlockingHandler EnableCommandBlocking()
        {
            _isCommandBlocking = true;
            return new CommandBlockingHandler();
        }
        
        public readonly struct CommandBlockingHandler : IDisposable
        {
            public void Dispose()
            {
                _isCommandBlocking = false;
            }
        }

        #region Entry

        public static void Entry<TView>() where TView : class, IView
        {
            if (_entryView != null) throw new Exception("entry view is only one");
            EnqueueCommand(new PushEntryRouteGroupCommand<TView>());
        }

        public class PushEntryRouteGroupCommand<TView> : PushRouteGroupCommandBase
            where TView : class, IView
        {
            protected override IView GetNextView(ISystemContext context)
            {
                _entryView = context.ResolveView<TView>();
                return _entryView;
            }
        }

        public static bool IsVisibleEntryView()
        {
#if UNITY_2021_2_OR_NEWER
            return _entryView is {Alpha: > 0};
#else
            return _entryView != null && _entryView.Alpha > 0;
#endif
        }

        public static UniTask WaitUntilNextEntry()
        {
            if (_sharedInstance == null)
            {
                throw new NullReferenceException("UISystem.Instance");
            }

            return UniTask.WaitUntil(IsVisibleEntryView);
        }

        #endregion

        public static void To(IView view)
        {
            SafeEnqueueCommand(new PushRouteGroupCommand(view));
        }

        public static void To<TView>() where TView : class, IView
        {
            SafeEnqueueCommand(new PushRouteGroupCommand<TView>());
        }

        public static void Execute(ICommand command)
        {
            SafeEnqueueCommand(command);
        }

        public static void Log(string message)
        {
            SafeEnqueueCommand(new LogCommand(message));
        }

        private class LogCommand : ICommand
        {
            private readonly string _log;

            public LogCommand(string message)
            {
                _log = message;
            }

            public UniTask Execute(ISystemContext context)
            {
                Debug.Log(_log);
                return UniTask.CompletedTask;
            }
        }

        public static void To(IView view, IManipulator manipulator)
        {
            SafeEnqueueCommand(new PushRouteGroupCommandWithManipulator(view, manipulator));
        }

        public static void To<TView>(IManipulator manipulator) where TView : class, IView
        {
            SafeEnqueueCommand(new PushRouteGroupCommandWithManipulator<TView>(manipulator));
        }

        public static void Back()
        {
            Back(false);
        }

        public static void Back(bool isThrowEmpty)
        {
            if (IsBlocking) return;
            if (_entryView == null)
            {
                if (!_sharedInstance.View.Any())
                {
                    if (isThrowEmpty)
                    {
                        throw new EmptySafetyAnyException();
                    }

                    return;
                }

                EnqueueCommand(new PopRouteGroupCommand(false));
            }
            else
            {
                if (!_sharedInstance.View.SafetyAny())
                {
                    if (isThrowEmpty)
                    {
                        throw new EmptySafetyAnyException();
                    }

                    return;
                }

                EnqueueCommand(new PopRouteGroupCommand());
            }
        }


        UniTask ICommandContext.Execute(ICommand command)
        {
            return command.Execute(this);
        }

        UniTask ICommandContext.To(IView view)
        {
            return new PushRouteGroupCommand(view).Execute(this);
        }

        UniTask ICommandContext.To(IView view, IManipulator manipulator)
        {
            return new PushRouteGroupCommandWithManipulator(view, manipulator).Execute(this);
        }

        UniTask ICommandContext.Push(IView view) => new PushCommand(view).Execute(this);

        UniTask ICommandContext.Push(IView view, IManipulator manipulator) =>
            new PushWithManipulatorCommand(view, manipulator).Execute(this);

        UniTask ICommandContext.Replace(IView view) => new ReplaceCommand(view).Execute(this);
        UniTask ICommandContext.Pop() => new PopCommand().Execute(this);

        UniTask ICommandContext.Transition(IView view) => new TransitionCommand(view).Execute(this);
        UniTask ICommandContext.TransitionPop() => new TransitionPopCommand().Execute(this);
    }

    public class EmptySafetyAnyException : Exception
    {
    }
}