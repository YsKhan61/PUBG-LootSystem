using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    public class WeaponSlotUI : MonoBehaviour, IDropHandler
    {
        [Header("Broadcast to")]
        [SerializeField, FormerlySerializedAs("m_OnGunItemUIDroppedInInventoryUIEvent")]
        GunItemIntEventChannelSO m_OnGunItemUIDroppedInWeaponSlotUIEvent;

        [Space(10)]

        [SerializeField, Tooltip("The index number of this slot. 0 - Primary weapon, 1 - Secondary weapon etc.")]
        int m_SlotIndex;

        [SerializeField, Tooltip("The items UI types that are allowed to drop in here")]
        ItemUIType[] m_typeToStore;

        [SerializeField]
        WeaponInventoryUI m_WeaponInventoryUI;

        public WeaponItemUI WeaponItemUI { get; private set; }
        
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                TryDropItem(eventData.pointerDrag);
            }
        }

        public void TryDropItem(GameObject item)
        {
            if (item.TryGetComponent(out WeaponItemUI itemUI))
            {
                if (itemUI.Item == null)
                {
                    return;
                }

                foreach (ItemUIType type in m_typeToStore)
                {
                    if (itemUI.ItemData.UIType == type)
                    {
                        int index = m_WeaponInventoryUI.GetIndexOfWeaponItemUI(itemUI);
                        if (index < 0)
                        {
                            Debug.LogError("Should not happen: Each weapon item UI must have a slot registered in WeaponInventoryUI");
                        }

                        m_WeaponInventoryUI.SwapWeaponItemUIs(index, m_SlotIndex);

                        /*item.transform.SetParent(transform);
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        itemUI.IsDragSuccess = true;

                        m_Icon.sprite = itemUI.ItemData.IconSprite;
                        m_NameText.text = itemUI.ItemData.name;

                        // This event is raised, for different occasions.
                        // For example: if a gun is placed from Inventory Bag Panel, to the Gun Slot Panel.
                        // The event will be raised, and the gun icon will be added to the Gun Slot Panel.
                        // Also the respective gun will be added to User's hand.
                        m_OnGunItemUIDroppedInWeaponSlotUIEvent?.RaiseEvent(itemUI.Item as GunItem, m_SlotIndex);

                        Destroy(itemUI);*/
                        return;
                    }
                }
            }

            else if (item.TryGetComponent(out itemUI))
            {
                // This is ItemUI that contains weapon item.
                // Later if we implement a feature to showcase items nearby in inventory,
                // then if a weapon is nearby, a itemUI will be created and added to the Nearby Panel (it is not implemented yet).
                // Then we can drag and drop the weapon Item UI(ItemUI) to the weapon slot.
            }
        }

        /// <summary>
        /// This method is used to add ItemUI to slot directly after pickup.
        /// </summary>
        /// <param name="item"></param>
        public void TryAddItemUIToSlotUI(WeaponItemUI itemUI)
        {
            foreach (ItemUIType type in m_typeToStore)
            {
                if (itemUI.ItemData.UIType == type)
                {
                    itemUI.transform.SetParent(transform);
                    itemUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    itemUI.IsDragSuccess = true;

                    /*m_Icon.sprite = itemUI.ItemData.IconSprite;
                    m_NameText.text = itemUI.ItemData.name;*/
                    WeaponItemUI = itemUI;

                    // This event is raised, for different occasions.
                    // For example: if a gun is placed from Inventory Bag Panel, to the Gun Slot Panel.
                    // The event will be raised, and the gun icon will be added to the Gun Slot Panel.
                    // Also the respective gun will be added to User's hand.
                    m_OnGunItemUIDroppedInWeaponSlotUIEvent?.RaiseEvent(itemUI.Item, m_SlotIndex);
                    return;
                }
            }
        }

        public void TryRemoveItemUIFromSlotUI()
        {
            if (WeaponItemUI == null)
            {
                return;
            }

            /*m_Icon.sprite = null;
            m_NameText.text = string.Empty;*/
            WeaponItemUI = null;
        }
    }
}