using System;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "GunItemIntEventChannel", menuName = "ScriptableObjects/Event Channels/GunItemIntEventChannelSO")]
    public class WeaponItemIntEventChannelSO : ScriptableObject
    {
        public event Action<WeaponItem, int> OnEventRaised;

        public void RaiseEvent(WeaponItem item, int slotIndex)
        {
            OnEventRaised?.Invoke(item, slotIndex);
        }
    }
}
