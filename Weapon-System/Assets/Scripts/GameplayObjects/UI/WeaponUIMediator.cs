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
        WeaponItemIntEventChannelSO m_OnWeaponItemAddedToInventoryEvent;

        [Header("Broadcast to")]

        [SerializeField, Tooltip("When an WeaponItemUI is going to be removed from WeaponInventoryUI, this event is invoked before that happens")]
        WeaponItemIntEventChannelSO m_OnBeforeWeaponItemUIRemovedEvent;


        [SerializeField, Tooltip("When an WeaponItemUI is removed from WeaponInventoryUI, this event is invoked after that")]
        WeaponItemIntEventChannelSO m_OnAfterWeaponItemUIRemovedEvent;

        [SerializeField, Tooltip("When two WeaponItemUI's are swapped with each other, this event is invoked")]
        IntIntEventChannelSO m_OnWeaponItemUISwappedEvent;

        [Space(10)]

        [SerializeField]
        WeaponItemUI[] m_WeaponItemUIs;



        private void Start()
        {
            m_OnWeaponItemAddedToInventoryEvent.OnEventRaised += AddWeaponItemUIToInventoryUI;
        }

        private void OnDestroy()
        {
            m_OnWeaponItemAddedToInventoryEvent.OnEventRaised -= AddWeaponItemUIToInventoryUI;
        }

        internal void BroadcastOnBeforeWeaponItemUIRemovedEvent(WeaponItem item, int slotIndex)
        {
            m_OnBeforeWeaponItemUIRemovedEvent.RaiseEvent(item, slotIndex);
        }

        internal void BroadcastOnAfterWeaponItemUIRemovedEvent(WeaponItem item, int slotIndex)
        {
            m_OnAfterWeaponItemUIRemovedEvent.RaiseEvent(item, slotIndex);
        }

        internal void BroadcastWeaponItemUIsSwappedEvent(int indexOfDroppedWeaponItemUI, int indexOfWeaponSlotUI)
        {
            m_OnWeaponItemUISwappedEvent.RaiseEvent(indexOfDroppedWeaponItemUI, indexOfWeaponSlotUI);
        }

        public bool TryGetWeaopnItemFromWeaponInventoryUI(int index, out WeaponItem gun)
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
        public void DropWeaponItemUIToSlot(WeaponItemUI itemUI, Transform slotTransform, int slotIndex)
        {
            itemUI.transform.SetParent(slotTransform);
            itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemUI.IsDragSuccess = true;

            itemUI.SetSlotIndex(slotIndex);
        }

        /// <summary>
        /// When an weapon is picked up, this method is called to add the weapon to the inventory UI
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        private void AddWeaponItemUIToInventoryUI(WeaponItem item, int index)
        {

            if (!TryGetWeaponItemUIFromSlotIndex(index, out WeaponItemUI weaponItemUI))
            {
                Debug.LogError("WeaponItemUI not found for the index: " + index);
                return;
            }

            if (weaponItemUI.StoredGunItem != null)
            {
                // If a weapon is already in the inventory, then return for now.
                // This is a temporary solution, as we are not removing the gun from the inventory.
                // We are just adding the gun to the inventory.
                // Later, we gonna implement the mechanics applied in PUBG
                return;
            }

            weaponItemUI.SetItemDataAndShow(item);
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