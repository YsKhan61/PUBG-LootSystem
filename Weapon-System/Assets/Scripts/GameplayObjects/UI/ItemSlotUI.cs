using UnityEngine;
using UnityEngine.EventSystems;


namespace Weapon_System.GameplayObjects.UI
{
    public class ItemSlotUI : MonoBehaviour, IDropHandler
    {
        [SerializeField, Tooltip("The parent under which this item will be added")]
        Transform m_ParentTransform;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                TryDropItemInSlot(eventData.pointerDrag);
            }
        }

        void TryDropItemInSlot(GameObject item)
        {
            if (item.TryGetComponent(out ItemUI itemUI))
            {
                item.transform.SetParent(m_ParentTransform);
                itemUI.IsDragSuccess = true;
                itemUI.Show();
                itemUI.BlockRaycast();
                return;
            }
        }
    }
}