using UnityEngine;
using UnityEngine.EventSystems;
using Weapon_System.Utilitites;

namespace Weapon_System.GameplayObjects.UI
{
    public class ItemSlotUI : MonoBehaviour, IDropHandler
    {
        [SerializeField, Tooltip("The items that are allowed to drop in here")]
        ItemTagSO[] m_ItemTags;
        
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                TryDropItem(eventData.pointerDrag);
            }
        }

        void TryDropItem(GameObject item)
        {
            if (item.TryGetComponent(out ItemDragDropUI itemDragDropUI))
            {
                foreach (var itemTag in m_ItemTags)
                {
                    if (itemDragDropUI.ItemTag == itemTag)
                    {
                        item.transform.SetParent(transform);
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        itemDragDropUI.IsDragSuccess = true;
                        return;
                    }
                }
            }
        }
    }
}