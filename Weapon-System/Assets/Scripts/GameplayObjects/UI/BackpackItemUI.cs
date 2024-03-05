using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class BackpackItemUI : MonoBehaviour, IPointerDownHandler, IDropHandler
    {
        [SerializeField]
        ItemUIType m_ItemUIType;
        public ItemUIType ItemUIType => m_ItemUIType;

        [SerializeField]
        Image m_Icon;

        InventoryItem m_StoredItem;
        public InventoryItem StoredItem => m_StoredItem;

        private Color m_DefaultColor;

        private void Start()
        {
            m_DefaultColor = m_Icon.color;
            ResetData();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Drop backpack item
            }
        }


        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            if (eventData.pointerDrag.TryGetComponent(out ItemUI droppedItemUI))
            {
                // Check if it's UIType is same as this UIType
                // If yes, store backpack
            }
        }

        public void SetData(InventoryItem item)
        {
            m_StoredItem = item;
            m_Icon.sprite = item.ItemData.IconSprite;
            m_Icon.color = new Color(1, 1, 1, 1);
        }

        public void ResetData()
        {
            m_StoredItem = null;
            m_Icon.sprite = null;
            m_Icon.color = m_DefaultColor;
        }
    }
}
