using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the Weapon related UIs (WeaponItemUIs and WeaponSlotUIs) in the WeaponSlotUIs.
    /// </summary>
    public class WeaponUIMediator : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("When an Weapon item is added to the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToWeaponInventoryEvent;

        [SerializeField, Tooltip("When an Weapon item is removed from the inventory, this event is invoked")]
        WeaponItemIntEventChannelSO m_OnWeaponItemRemovedFromWeaponInventoryEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other in the inventory, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemSwappedInInventoryEvent;


        [Space(10)]

        [SerializeField]
        ItemUITagSO m_ItemUITag;
        public ItemUITagSO ItemUITag => m_ItemUITag;

        [SerializeField]
        Canvas m_Canvas;
        public Transform CanvasTransform => m_Canvas.transform;

        [SerializeField]
        WeaponItemUI[] m_WeaponItemUIs;

        [SerializeField]
        ItemUserHand m_ItemUserHand;


        private void Start()
        {
            m_OnWeaponItemAddedToWeaponInventoryEvent.OnEventRaised += OnWeaponItemAddedToWeaponInventoryEvent;
            m_OnWeaponItemRemovedFromWeaponInventoryEvent.OnEventRaised += OnWeaponItemRemovedFromWeaponInventoryEvent;
            m_OnWeaponItemSwappedInInventoryEvent.OnEventRaised += OnWeaponItemSwappedInInventoryEvent;
        }

        private void OnDestroy()
        {
            m_OnWeaponItemAddedToWeaponInventoryEvent.OnEventRaised -= OnWeaponItemAddedToWeaponInventoryEvent;
            m_OnWeaponItemRemovedFromWeaponInventoryEvent.OnEventRaised -= OnWeaponItemRemovedFromWeaponInventoryEvent;
            m_OnWeaponItemSwappedInInventoryEvent.OnEventRaised -= OnWeaponItemSwappedInInventoryEvent;
        }

        internal void TryAddWeaponAndDestroyItemUI(ItemUI droppedItemUI)
        {
            bool success = (droppedItemUI.StoredItem as WeaponItem).TryStoreAndCollect(m_ItemUserHand);
            if (success)
            {
                droppedItemUI.ReleaseSelfToPool();
            }
        }

        /// <remarks>
        /// NOTE to check out the same methods of WeaponUIMediator class and from InventoryUI class, before modifying this method.
        /// Weapon can be removed from inventory by dropping the weapon item UI to vicinity
        /// or right clicking on the weapon item UI in the inventory
        /// </remarks>
        internal bool TryRemoveWeaponItemFromInventory(WeaponItem weaponItem, int index)
        {
            if (weaponItem == null)
            {
                return false;
            }

            return weaponItem.TryRemoveAndDrop();
        }

        internal void SwapWeaponItemsInInventory(int leftIndex, int rightIndex)
        {
            m_ItemUserHand.SwapWeaponsInWeaponStorage(leftIndex, rightIndex);
        }

        internal bool TryGetWeaopnItemFromWeaponInventoryUI(int index, out WeaponItem gun)
        {
            gun = null;

            if (!TryGetWeaponItemUIFromSlotIndex(index, out WeaponItemUI weaponItemUI))
            {
                return false;
            }
            gun = weaponItemUI.StoredGunItem;
            return gun != null;
        }

        /// <summary>
        /// This method drops an already existing weapon item UI to the slot of given slot index
        /// </summary>
        /// <param name="itemUI"></param>
        /// <param name="slotTransform"></param>
        /// <param name="slotIndex"></param>
        private void DropWeaponItemUIToSlot(WeaponItemUI itemUI, Transform slotTransform, int slotIndex)
        {
            itemUI.transform.SetParent(slotTransform);
            itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemUI.IsDragSuccess = true;

            itemUI.SetSlotIndex(slotIndex);
        }

        /// <summary>
        /// When an weapon is picked up, this method is called to add the weapon to the inventory UI
        /// </summary>
        /// <param name="weaponItem"></param>
        /// <param name="index"></param>
        private void OnWeaponItemAddedToWeaponInventoryEvent(WeaponItem weaponItem, int index)
        {

            if (!TryGetWeaponItemUIFromSlotIndex(index, out WeaponItemUI weaponItemUI))
            {
                Debug.LogError("WeaponItemUI not found for the index: " + index);
                return;
            }

            // It's not needed to check if the weaponItemUI is already holding a weapon item
            // As we are overriding the weapon item in the inventory by dropping the old one.
            /*if (weaponItemUI.StoredGunItem != null)
            {
                // If a weapon is already in the inventory, then return for now.
                // This is a temporary solution, as we are not removing the gun from the inventory.
                // We are just adding the gun to the inventory.
                // Later, we gonna implement the mechanics applied in PUBG
                return;
            }*/

            weaponItemUI.SetItemDataAndShow(weaponItem);
        }

        private void OnWeaponItemRemovedFromWeaponInventoryEvent(WeaponItem weaponItem, int index)
        {
            if (!TryGetWeaponItemUIFromSlotIndex(index, out WeaponItemUI weaponItemUI))
            {
                Debug.LogError("WeaponItemUI not found for the index: " + index);
                return;
            }

            weaponItemUI.ResetDataAndHideGunItemUI();
        }

        private void OnWeaponItemSwappedInInventoryEvent(int leftIndex, int rightIndex)
        {
            if (!TryGetWeaponItemUIFromSlotIndex(leftIndex, out WeaponItemUI leftItemUI))
            {
                Debug.LogError("WeaponItemUI not found for the index: " + leftIndex);
                return;
            }

            if (!TryGetWeaponItemUIFromSlotIndex(rightIndex, out WeaponItemUI rightItemUI))
            {
                Debug.LogError("WeaponItemUI not found for the index: " + rightIndex);
                return;
            }

            Transform parentOfLeftItemUI = leftItemUI.LastParent;
            int slotIndexOfLeftItemUI = leftItemUI.SlotIndex;

            DropWeaponItemUIToSlot(leftItemUI, rightItemUI.transform.parent, rightItemUI.SlotIndex);
            DropWeaponItemUIToSlot(rightItemUI, parentOfLeftItemUI, slotIndexOfLeftItemUI);
        }

        private bool TryGetWeaponItemUIFromSlotIndex(int index, out WeaponItemUI itemUI)
        {
            itemUI = null;
            foreach (WeaponItemUI weaponItemUI in m_WeaponItemUIs)
            {
                if (weaponItemUI.SlotIndex == index)
                {
                    itemUI = weaponItemUI;
                    return true;
                }
            }

            return false;
        }
    }
}