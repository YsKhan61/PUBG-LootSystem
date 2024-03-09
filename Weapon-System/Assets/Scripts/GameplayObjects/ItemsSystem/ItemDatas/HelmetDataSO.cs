using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "HelmetData", menuName = "ScriptableObjects/ItemDatas/HelmetDataSO")]
    public class HelmetDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("When a helmet is added to the inventory, this event is invoked")]
        internal HelmetItemEventChannelSO OnHelmetItemAddedToInventory;

        [SerializeField, Tooltip("When a helmet is removed from the inventory, this event is invoked")]
        internal HelmetItemEventChannelSO OnHelmetItemRemovedFromInventory;
    }

}