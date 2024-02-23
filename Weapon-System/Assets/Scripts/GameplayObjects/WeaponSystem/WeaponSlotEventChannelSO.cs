using UnityEngine;
using Weapon_System.Utilities;

namespace Weapon_System.GameplayObjects.WeaponSystem
{
    [CreateAssetMenu(fileName = "WeaponSlotEventChannel", menuName = "ScriptableObjects/Event Channels/WeaponSlotEventChannelSO")]
    public class WeaponSlotEventChannelSO : EventChannelBaseSO<WeaponSlotManager.SlotLabel>
    {
        public WeaponSlotManager.SlotLabel Label { get; private set; }
        public override void RaiseEvent(WeaponSlotManager.SlotLabel label)
        {
            Label = label;
            OnEventRaised?.Invoke(label);
        }
    }
}