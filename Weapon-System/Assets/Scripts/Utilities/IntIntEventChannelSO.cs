using System;
using UnityEngine;

namespace Weapon_System.Utilities
{
    [CreateAssetMenu(fileName = "IntIntEventChannel", menuName = "ScriptableObjects/Event Channels/IntIntEventChannelSO")]
    public class IntIntEventChannelSO : ScriptableObject
    {
        public event Action<int, int> OnEventRaised;

        public void RaiseEvent(int value1, int value2)
        {
            OnEventRaised?.Invoke(value1, value2);
        }
    }
}