using System;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "WeaponItemIntEventChannel", menuName = "ScriptableObjects/Event Channels/WeaponItemIntEventChannelSO")]
    public class WeaponItemIntEventChannelSO : ScriptableObject
    {
        public event Action<WeaponItem, int> OnEventRaised;

        public void RaiseEvent(WeaponItem item, int slotIndex)
        {
            OnEventRaised?.Invoke(item, slotIndex);
        }
    }
}
