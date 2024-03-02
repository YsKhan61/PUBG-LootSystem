using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "InventoryItemEventChannel", menuName = "ScriptableObjects/Event Channels/InventoryItemEventChannelSO")]
    public class InventoryItemEventChannelSO : EventChannelBaseSO<InventoryItem>
    {
    }
}
