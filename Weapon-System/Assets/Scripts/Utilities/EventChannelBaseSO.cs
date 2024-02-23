using System;
using UnityEngine;


namespace Weapon_System.Utilities
{
    public abstract class EventChannelBaseSO<T> : ScriptableObject
    {
        public Action<T> OnEventRaised;
        public abstract void RaiseEvent(T value);
    }
}
