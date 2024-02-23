using System;
using UnityEngine;


namespace Weapon_System.Utilities
{
    public abstract class EventChannelBaseSO<T> : ScriptableObject
    {
        [SerializeField]
        protected T m_Value;
        public T Value { get => m_Value; }

        public Action<T> OnEventRaised;

        public virtual void SetValue(T value)
        {
            m_Value = value;
        }

        public virtual void RaiseEvent()
        {
            OnEventRaised?.Invoke(m_Value);
        }

        public virtual void SetValueAndRaiseEvent(T value)
        {
            m_Value = value;
            OnEventRaised?.Invoke(m_Value);
        }
    }
}
