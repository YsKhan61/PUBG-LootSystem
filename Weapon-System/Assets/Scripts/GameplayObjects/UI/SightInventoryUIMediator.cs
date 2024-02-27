
using System;
using UnityEngine;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the sight attachment UIs (SightItemUIs and SightSlotUIs) in the WeaponSlotUIs.
    /// </summary>
    public class SightInventoryUIMediator : MonoBehaviour
    {
        [Serializable]
        class SightItemUISlotUIPair
        {
            [SerializeField]
            internal SightItemUI sightItemUI;
            [SerializeField]
            internal SightSlotUI sightSlotUI;
        }

        [Space(10)]

        [SerializeField]
        SightItemUISlotUIPair[] m_SightItemUISlotUIPairs;

        [SerializeField]
        Inventory m_Inventory;

        [SerializeField]
        WeaponInventoryUIMediator m_WeaponInventoryUI;
        public WeaponInventoryUIMediator WeaponInventoryUI => m_WeaponInventoryUI;

        private void Start()
        {
            /*for (int i = 0; i < m_SightItemUISlotUIPairs.Length; i++)
            {
                m_SightItemUISlotUIPairs[i].sightItemUI.SetSlotIndex(i);
            }*/
        }


        /*/// <summary>
        /// Swap the weapon item UIs in the weapon inventory UI's weapon slots.
        /// </summary>
        /// <param name="indexOfDroppedWeaponItemUI">index of the WeaponItemUI that is being dropped</param>
        /// <param name="indexOfWeaponSlotUI">index of the WeaponSlotUI where the WeaponItemUI is being dropped</param>
        /// <param name="raiseEvent">True - raise an event after swap complete, False - don't raise event</param>
        /// <remarks>
        /// It would be more simpler if we had have only 2 slots,
        /// but if we want more slots, and swap UIs between them,
        /// then this approach is better.
        /// </remarks>
        public void SwapSightItemUIs(int indexOfDroppedWeaponItemUI, int indexOfWeaponSlotUI)
        {
            SightItemUISlotUIPair leftPair = m_SightItemUISlotUIPairs[indexOfDroppedWeaponItemUI];
            SightItemUISlotUIPair rightPair = m_SightItemUISlotUIPairs[indexOfWeaponSlotUI];

            // return if the weaponItemUIToDrop is the same one as in the slot
            if (leftPair.sightItemUI == rightPair.sightItemUI)
                return;

            rightPair.sightSlotUI.TryAddItemUIToSlotUI(leftPair.sightItemUI);
            leftPair.sightSlotUI.TryAddItemUIToSlotUI(rightPair.sightItemUI);

            SightItemUI temp = leftPair.sightItemUI;
            leftPair.sightItemUI = rightPair.sightItemUI;
            rightPair.sightItemUI = temp;
        }

        /// <summary>
        /// Add the SightItemUI to the SightSlotUI of the slotIndex given.
        /// </summary>
        /// <param name="item">The SightItemUI need to be added</param>
        /// <param name="slotIndex">Index of the destination SightSlotUI</param>
        public void AddSightItemUIToSightSlotUI(SightItemUI itemUI, int slotIndex)
        {
            if (!TryGetSightSlotUIFromIndex(slotIndex, out SightSlotUI slotUI))
            {
                return;
            }

            slotUI.TryAddItemUIToSlotUI(itemUI);
        }

        /// <summary>
        /// Remove the SightItemUI from it's SightSlotUI
        /// </summary>
        /// <param name="item">The GunItem to be removed</param>
        public void RemoveSightItemUIFromSightSlotUI(SightAttachmentItem item)
        {
            if (!TryGetIndexOfSightItemUIFromGunItem(item, out int index))
                return;

            m_SightItemUISlotUIPairs[index].sightSlotUI.TryRemoveItemUIFromSlotUI();
        }*/

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

        /*private bool TryGetIndexOfSightItemUIFromGunItem(SightAttachmentItem item, out int index)
        {
            index = -1;
            for (int i = 0, length = m_SightItemUISlotUIPairs.Length; i < length; i++)
            {
                if (m_SightItemUISlotUIPairs[i].sightItemUI.StoredSightItem == item)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        private bool TryGetSightSlotUIFromIndex(int index, out SightSlotUI slotUI)
        {
            slotUI = null;
            if (index < 0 || index >= m_SightItemUISlotUIPairs.Length)
            {
                Debug.LogError("Index out of range");
                return false;
            }
            slotUI = m_SightItemUISlotUIPairs[index].sightSlotUI;
            return slotUI != null;
        }*/
    }
}
