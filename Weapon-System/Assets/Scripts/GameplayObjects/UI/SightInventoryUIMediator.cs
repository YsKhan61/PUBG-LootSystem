
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the sight attachment UIs (SightItemUIs and SightSlotUIs) in the WeaponSlotUIs.
    /// </summary>
    public class SightInventoryUIMediator : MonoBehaviour
    {
        [SerializeField]
        Inventory m_Inventory;

        [SerializeField]
        WeaponInventoryUIMediator m_WeaponInventoryUI;
        public WeaponInventoryUIMediator WeaponInventoryUI => m_WeaponInventoryUI;

        public void AddSightItemToInventory(InventoryItem item)
        {
            m_Inventory.AddItemToInventory(item);
        }

        public void RemoveSightItemFromInventory(InventoryItem item)
        {
            m_Inventory.RemoveInventoryItem(item);
        }

        /// <summary>
        /// Drop the other SightItemUI to this SightItemUI
        /// </summary>
        /// <param name="itemUI">the other SightItemUI</param>
        public void DropOtherSightItemUIToSlot(SightItemUI itemUI, Transform slotTransform, int slotIndex)
        {
            itemUI.transform.SetParent(slotTransform);
            itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemUI.IsDragSuccess = true;

            itemUI.SetSlotIndex(slotIndex);
        }
    }
}
