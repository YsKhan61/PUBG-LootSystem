using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    public class HelmetItemUI : MonoBehaviour, IPointerDownHandler, IDropHandler
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("When a helmet is added to the inventory, this event is invoked")]
        HelmetItemEventChannelSO m_OnHelmetItemAddedToInventory;

        [SerializeField, Tooltip("When a helmet is removed from the inventory, this event is invoked")]
        HelmetItemEventChannelSO m_OnHelmetItemRemovedFromInventory;

        [Space(10)]

        [SerializeField]
        ItemUITagSO m_ItemUITag;
        public ItemUITagSO ItemUITag => m_ItemUITag;

        [SerializeField]
        Image m_Icon;

        [SerializeField]
        ItemUserHand m_ItemUserHand;

        HelmetItem m_StoredHelmet;
        public HelmetItem StoredHelmet => m_StoredHelmet;

        private Color m_DefaultColor;


        private void Start()
        {
            m_OnHelmetItemAddedToInventory.OnEventRaised += SetData;
            m_OnHelmetItemRemovedFromInventory.OnEventRaised += ResetData;

            m_DefaultColor = m_Icon.color;
            ResetData(null);
        }

        private void OnDestroy()
        {
            m_OnHelmetItemAddedToInventory.OnEventRaised -= SetData;
            m_OnHelmetItemRemovedFromInventory.OnEventRaised -= ResetData;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Check if backpack is present
                if (m_StoredHelmet == null)
                    return;

                // If yes, drop backpack
                m_StoredHelmet.TryRemoveAndDrop();
            }
        }


        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            if (eventData.pointerDrag.TryGetComponent(out ItemUI droppedItemUI))
            {
                // Check if it's UIType is same as this UIType
                if (droppedItemUI.StoredItem.ItemData.UITag != m_ItemUITag)
                {
                    return;
                }

                // If no, try store backpack
                bool success = (droppedItemUI.StoredItem as HelmetItem).TryStoreAndCollect(m_ItemUserHand);
                if (success)
                {
                    droppedItemUI.ReleaseSelfToPool();
                }
            }
        }

        public void SetData(HelmetItem item)
        {
            m_StoredHelmet = item;
            m_Icon.sprite = item.ItemData.IconSprite;
            m_Icon.color = new Color(1, 1, 1, 1);
        }

        public void ResetData(HelmetItem _)
        {
            m_StoredHelmet = null;
            m_Icon.sprite = null;
            m_Icon.color = m_DefaultColor;
        }
    }
}
