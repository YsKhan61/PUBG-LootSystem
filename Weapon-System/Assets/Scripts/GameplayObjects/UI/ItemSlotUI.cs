using UnityEngine;
using UnityEngine.EventSystems;
using Weapon_System.Utilities;

namespace Weapon_System.GameplayObjects.UI
{
    public class ItemSlotUI : MonoBehaviour, IDropHandler
    {
        [Header("Broadcast to")]
        [SerializeField]
        ItemTagEventChannelSO m_OnItemDroppedEvent;

        [Space(10)]

        [SerializeField, Tooltip("The items that are allowed to drop in here")]
        ItemTagSO[] m_ItemTags;

        ItemUI m_ItemUI;
        
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                TryDropItem(eventData.pointerDrag);
            }
        }

        void TryDropItem(GameObject item)
        {
            if (m_ItemUI != null) 
            {
                return;
            }

            if (item.TryGetComponent(out ItemUI itemUI))
            {
                foreach (ItemTagSO itemTag in m_ItemTags)
                {
                    if (itemUI.ItemTag == itemTag)
                    {
                        item.transform.SetParent(transform);
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        itemUI.IsDragSuccess = true;
                        m_ItemUI = itemUI;
                        m_OnItemDroppedEvent.SetValueAndRaiseEvent(itemTag);
                        return;
                    }
                }
            }
        }
    }
}