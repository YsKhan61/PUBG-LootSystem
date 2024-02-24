using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;

namespace Weapon_System.GameplayObjects.UI
{
    public class ItemSlotUI : MonoBehaviour, IDropHandler
    {
        [Header("Broadcast to")]
        [SerializeField]
        CommonItemEventChannelSO m_OnItemUIDroppedInInventoryUIEvent;

        [Space(10)]

        [SerializeField, Tooltip("The items UI types that are allowed to drop in here")]
        ItemUIType[] m_typeToStore;

        [SerializeField]
        Image m_Icon;
        
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                TryDropItem(eventData.pointerDrag);
            }
        }

        void TryDropItem(GameObject item)
        {
            if (item.TryGetComponent(out ItemUI itemUI))
            {
                foreach (ItemUIType type in m_typeToStore)
                {
                    if (itemUI.ItemData.UIType == type)
                    {
                        item.transform.SetParent(transform);
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        itemUI.IsDragSuccess = true;

                        m_Icon.sprite = itemUI.ItemData.IconSprite;

                        // This event is raised, for different occasions.
                        // For example: if a gun is placed from Inventory Bag Panel, to the Gun Slot Panel.
                        // The event will be raised, and the gun icon will be added to the Gun Slot Panel.
                        // Also the respective gun will be added to User's hand.
                        m_OnItemUIDroppedInInventoryUIEvent?.SetValueAndRaiseEvent(itemUI.Item);
                        Destroy(itemUI);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to add ItemUI to slot directly after pickup.
        /// </summary>
        /// <param name="itemData"></param>
        public void TryAddItemToSlotUI(ItemDataSO itemData)
        {
            foreach (ItemUIType type in m_typeToStore)
            {
                if (itemData.UIType == type)
                {
                    m_Icon.sprite = itemData.IconSprite;

                    return;
                }
            }
        }
    }
}