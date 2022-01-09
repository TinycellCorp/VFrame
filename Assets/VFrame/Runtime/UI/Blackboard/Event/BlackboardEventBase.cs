using VFrame.UI.Blackboard.Reference;
using UnityEngine;
using UnityEngine.Events;

namespace VFrame.UI.Blackboard.Event
{
    public abstract class BlackboardEventBase<TReference, T> : MonoBehaviour where TReference : ReferenceBase<T>
    {
        [SerializeField] private TReference reference;
        [SerializeField] private RegisterMode mode = RegisterMode.EnableAndDisable;
        [SerializeField] private UnityEvent<T> onUpdateValue = new UnityEvent<T>();

        public RegisterMode Mode
        {
            get => mode;
            set => mode = value;
        }

        public UnityEvent<T> OnUpdateValue
        {
            get => onUpdateValue;
            set => onUpdateValue = value;
        }


        private void Awake()
        {
            reference.RefreshHandler();
            if (mode == RegisterMode.AwakeAndDestroy) RegisterEvent();
        }

        private void OnEnable()
        {
            if (mode == RegisterMode.EnableAndDisable) RegisterEvent();
        }

        private void OnDisable()
        {
            if (mode == RegisterMode.EnableAndDisable) ClearEvent();
        }

        private void OnDestroy()
        {
            ClearEvent();
            reference.Dispose();
        }


        private void UpdateValue(T value)
        {
            onUpdateValue.Invoke(value);
        }

        private void RegisterEvent()
        {
            reference.ValueChanged += UpdateValue;
        }

        private void ClearEvent()
        {
            reference.ValueChanged -= UpdateValue;
        }
    }
}