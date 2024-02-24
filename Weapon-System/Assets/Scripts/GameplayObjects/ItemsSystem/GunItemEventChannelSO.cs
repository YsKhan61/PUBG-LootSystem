using System;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "GunItemEventChannel", menuName = "ScriptableObjects/Event Channels/GunItemEventChannelSO")]
    public class GunItemEventChannelSO : ScriptableObject
    {
        public event Action<GunItem, int> OnEventRaised;

        public void RaiseEvent(GunItem item, int slotIndex)
        {
            OnEventRaised?.Invoke(item, slotIndex);
        }
    }
}
