using UnityEngine;
using Weapon_System.GameplayObjects.WeaponSlot;

namespace Weapon_System.Utilities
{
    [CreateAssetMenu(fileName = "WeaponSlotEventChannel", menuName = "ScriptableObjects/Event Channels/WeaponSlotEventChannelSO")]
    public class WeaponSlotEventChannelSO : EventChannelBaseSO<WeaponSlotManager.Slots>
    {
        public override void RaiseEvent(WeaponSlotManager.Slots slot)
        {
            OnEventRaised?.Invoke(slot);
        }
    }
}