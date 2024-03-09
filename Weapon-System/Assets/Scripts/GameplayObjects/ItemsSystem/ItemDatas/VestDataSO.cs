using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "VestData", menuName = "ScriptableObjects/ItemDatas/VestDataSO")]
    public class VestDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("When a vest is added to the inventory, this event is invoked")]
        internal VestItemEventChannelSO OnVestItemAddedToInventory;

        [SerializeField, Tooltip("When a vest is removed from the inventory, this event is invoked")]
        internal VestItemEventChannelSO OnVestItemRemovedFromInventory;
    }

}