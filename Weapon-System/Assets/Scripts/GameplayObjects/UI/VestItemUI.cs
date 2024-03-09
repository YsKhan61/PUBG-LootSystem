using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    public class VestItemUI : MonoBehaviour, IPointerDownHandler, IDropHandler
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("When a backpack is added to the inventory, this event is invoked")]
        VestItemEventChannelSO m_OnVestItemAddedToInventory;

        [SerializeField, Tooltip("When a backpack is removed from the inventory, this event is invoked")]
        VestItemEventChannelSO m_OnVestItemRemovedFromInventory;

        [Space(10)]

        [SerializeField]
        ItemUITagSO m_ItemUITag;
        public ItemUITagSO ItemUITag => m_ItemUITag;

        [SerializeField]
        Image m_Icon;

        [SerializeField]
        ItemUserHand m_ItemUserHand;

        VestItem m_StoredVest;
        public VestItem StoredVest => m_StoredVest;

        private Color m_DefaultColor;


        private void Start()
        {
            m_OnVestItemAddedToInventory.OnEventRaised += SetData;
            m_OnVestItemRemovedFromInventory.OnEventRaised += ResetData;

            m_DefaultColor = m_Icon.color;
            ResetData(null);
        }

        private void OnDestroy()
        {
            m_OnVestItemAddedToInventory.OnEventRaised -= SetData;
            m_OnVestItemRemovedFromInventory.OnEventRaised -= ResetData;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Check if backpack is present
                if (m_StoredVest == null)
                    return;

                // If yes, drop backpack
                // m_ItemUserHand.TryRemoveAndDropVest(m_StoredVest);
                m_StoredVest.TryRemoveAndDrop();
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
                bool success = (droppedItemUI.StoredItem as VestItem).TryStoreAndCollect(m_ItemUserHand);
                if (success)
                {
                    droppedItemUI.ReleaseSelfToPool();
                }
            }
        }

        public void SetData(VestItem item)
        {
            m_StoredVest = item;
            m_Icon.sprite = item.ItemData.IconSprite;
            m_Icon.color = new Color(1, 1, 1, 1);
        }

        public void ResetData(VestItem _)
        {
            m_StoredVest = null;
            m_Icon.sprite = null;
            m_Icon.color = m_DefaultColor;
        }
    }
}
