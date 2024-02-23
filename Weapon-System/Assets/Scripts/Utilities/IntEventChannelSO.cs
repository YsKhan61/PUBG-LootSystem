using UnityEngine;

namespace Weapon_System.Utilities
{
    [CreateAssetMenu(fileName = "IntEventChannel", menuName = "ScriptableObjects/Event Channels/IntEventChannelSO")]
    public class IntEventChannelSO : EventChannelBaseSO<int>
    {
        public int Value { get; private set; }

        public override void RaiseEvent(int value)
        {
            Value = value;
            OnEventRaised?.Invoke(value);
        }
    }
}