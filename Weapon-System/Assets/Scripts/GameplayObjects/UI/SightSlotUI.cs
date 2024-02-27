using UnityEngine;
using UnityEngine.EventSystems;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    public class SightSlotUI : MonoBehaviour
    {
        [Space(10)]

        [SerializeField, Tooltip("The index number of this slot. 0 - Primary weapon's sight, 1 - Secondary weapon's sight.")]
        int m_SlotIndex;

        [SerializeField, Tooltip("The items UI types that are allowed to drop in here")]
        ItemUIType[] m_typeToStore;

        [SerializeField]
        WeaponInventoryUIMediator m_WeaponInventoryUI;
        public SightItemUI StoredSightItemUI { get; private set; }


        /// <summary>
        /// This method is used to add ItemUI to slot directly after pickup.
        /// </summary>
        /// <param name="item"></param>
        public void TryAddItemUIToSlotUI(SightItemUI itemUI)
        {
            foreach (ItemUIType type in m_typeToStore)
            {
                if (itemUI.ItemUIType == type)
                {
                    itemUI.transform.SetParent(transform);
                    itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    itemUI.IsDragSuccess = true;

                    StoredSightItemUI = itemUI;
                    StoredSightItemUI.SetSlotIndex(m_SlotIndex);

                    // ------------------------- NOTE ---------------------------------
                    // This event is raised, for different occasions.
                    // For example: if a gun is placed from Inventory Bag Panel, to the Gun Slot Panel.
                    // The event will be raised, and the gun icon will be added to the Gun Slot Panel.
                    // Also the respective gun will be added to User's hand.
                    // m_OnGunItemUIDroppedInWeaponSlotUIEvent?.RaiseEvent(itemUI.Item, m_SlotIndex);
                    return;
                }
            }
        }

        public void TryRemoveItemUIFromSlotUI()
        {
            if (StoredSightItemUI == null)
            {
                return;
            }

            StoredSightItemUI = null;
        }
    }
}