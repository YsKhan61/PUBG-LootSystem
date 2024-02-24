using System;
using UnityEngine;


namespace Weapon_System.Utilities
{
    public abstract class EventChannelBaseSO<T> : ScriptableObject
    {
        [SerializeField, NonSerialized]             // Made for Debug purpose, remove both later
        protected T m_Value;
        public T Value { get => m_Value; }

        public Action<T> OnEventRaised;

        /// <summary>
        /// This method only sets the value. Doesn't raise any event.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValue(T value)
        {
            m_Value = value;
        }

        /// <summary>
        /// This method raises the event but doesn't passes any value to the listeners.
        /// </summary>
        public virtual void RaiseEvent()
        {
            OnEventRaised?.Invoke(default);
        }

        /// <summary>
        /// This method raises the event and passes the value to the listeners.
        /// </summary>
        /// <param name="value"></param>
        public virtual void RaiseEvent(T value)
        {
            OnEventRaised?.Invoke(value);
        }

        /// <summary>
        /// This method sets the value and raises the event, that passes the value to the listeners.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValueAndRaiseEvent(T value)
        {
            m_Value = value;
            OnEventRaised?.Invoke(m_Value);
        }
    }
}
