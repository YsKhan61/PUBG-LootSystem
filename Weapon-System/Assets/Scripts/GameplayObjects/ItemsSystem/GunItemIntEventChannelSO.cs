using System;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "GunItemIntEventChannel", menuName = "ScriptableObjects/Event Channels/GunItemIntEventChannelSO")]
    public class GunItemIntEventChannelSO : ScriptableObject
    {
        public event Action<GunItem, int> OnEventRaised;

        public void RaiseEvent(GunItem item, int slotIndex)
        {
            OnEventRaised?.Invoke(item, slotIndex);
        }
    }
}
