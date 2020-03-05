using UnityEngine;

namespace Spellplague.Utility
{
    public abstract class BaseScriptableObject<T> : ScriptableObject
    {
        [SerializeField]
        protected T value = default;
        public virtual T Value
        {
            get => value;
            set
            {
                ValueChangedEvent?.Invoke(value);
                this.value = value;
            }
        }
        public delegate void ValueChanged(T value);
        public event ValueChanged ValueChangedEvent;

        [SerializeField]
        protected bool resetValueOnStart = true;
        [SerializeField]
        protected T valueToResetTo = default;
        public T OriginalValue { get => valueToResetTo; }

        protected virtual void OnEnable()
        {
            if (resetValueOnStart)
            {
                value = valueToResetTo;
            }
        }
    }
}