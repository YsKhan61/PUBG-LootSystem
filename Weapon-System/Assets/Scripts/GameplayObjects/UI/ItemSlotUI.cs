using UnityEngine;
using UnityEngine.EventSystems;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// When an ItemUI is dropped above a slot
    /// We first check the type of the slot,
    /// and then we try to drop the item in the slot if it's of different type
    /// Then we set the StoredSlotType of that ItemUI to the type of the slot it is being dropped into.
    /// </summary>
    public enum SlotType
    {
        Inventory,
        Vicinity
    }

    public class ItemSlotUI : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        SlotType m_SlotType;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            // If slot type is vicinity, then if we drag and drop a WeaponItemUI, then remove it from the inventory and drop the weapon item
            if (eventData.pointerDrag.TryGetComponent(out WeaponItemUI weaponItemUI))
            {
                if (m_SlotType != SlotType.Vicinity)
                    return;

                weaponItemUI.WeaponUIMediator.RemoveWeaponItemFromInventory(weaponItemUI.StoredGunItem, weaponItemUI.SlotIndex);
            }

            else if (eventData.pointerDrag.TryGetComponent(out ItemUI itemUI))
            {
                if (itemUI.Item is WeaponItem)
                {
                    itemUI.InventoryUI.AddWeaponItemToWeaponInventory((WeaponItem)itemUI.Item);
                    return;
                }

                itemUI.InventoryUI.OnItemUIDroppedOnSlotType(itemUI, m_SlotType);
            }
        }
    }
}