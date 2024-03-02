using UnityEngine;
using UnityEngine.EventSystems;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// When an ItemUI is dropped above a slot
    /// We first check the type of the slot,
    /// and then we try to drop the item in the slot if it's of different type
    /// Then we set the StoredSlotType of that ItemUI to the type of the slot it is being dropped into.
    /// </summary>
    public enum SlotType
    {
        Inventory,
        Viscinity
    }

    public class ItemSlotUI : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        SlotType m_SlotType;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;
            
            if (eventData.pointerDrag.TryGetComponent(out ItemUI itemUI))
            {
                itemUI.InventoryUI.OnItemUIDroppedOnSlotType(itemUI, m_SlotType);
            }
        }
    }
}