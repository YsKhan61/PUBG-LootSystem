using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    public class BackpackItemUI : MonoBehaviour, IPointerDownHandler, IDropHandler
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("When a backpack is added to the inventory, this event is invoked")]
        BackpackItemEventChannelSO m_OnBackpackItemAddedToInventory;

        [SerializeField, Tooltip("When a backpack is removed from the inventory, this event is invoked")]
        BackpackItemEventChannelSO m_OnBackpackItemRemovedFromInventory;

        [Space(10)]

        [SerializeField]
        ItemUITagSO m_ItemUITag;
        public ItemUITagSO ItemUITag => m_ItemUITag;

        [SerializeField]
        Image m_Icon;

        [SerializeField]
        InventoryUI m_InventoryUI;

        BackpackItem m_StoredBackpack;
        public BackpackItem StoredBackpack => m_StoredBackpack;

        private Color m_DefaultColor;

        private void Start()
        {
            m_OnBackpackItemAddedToInventory.OnEventRaised += SetData;
            m_OnBackpackItemRemovedFromInventory.OnEventRaised += ResetData;

            m_DefaultColor = m_Icon.color;
            ResetData(null);
        }

        private void OnDestroy()
        {
            m_OnBackpackItemAddedToInventory.OnEventRaised -= SetData;
            m_OnBackpackItemRemovedFromInventory.OnEventRaised -= ResetData;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Check if backpack is present
                if (m_StoredBackpack == null)
                    return;

                // If yes, drop backpack
                m_InventoryUI.TryRemoveAndDropBackpackFromInventory(m_StoredBackpack);
            }
        }


        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            if (eventData.pointerDrag.TryGetComponent(out ItemUI droppedItemUI))
            {
                // Check if it's UIType is same as this UIType
                if (droppedItemUI.StoredItem.ItemData.UITag == m_ItemUITag)
                {
                    // If no, try store backpack
                    m_InventoryUI.TryAddBackpackAndDestroyItemUI(droppedItemUI);
                }
            }
        }

        public void SetData(BackpackItem item)
        {
            m_StoredBackpack = item;
            m_Icon.sprite = item.ItemData.IconSprite;
            m_Icon.color = new Color(1, 1, 1, 1);
        }

        public void ResetData(BackpackItem _)
        {
            m_StoredBackpack = null;
            m_Icon.sprite = null;
            m_Icon.color = m_DefaultColor;
        }
    }
}
