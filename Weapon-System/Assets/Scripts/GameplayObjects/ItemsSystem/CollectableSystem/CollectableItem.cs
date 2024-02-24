using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    ///  The items that can be collected and stored in inventory, and later can be dropped as well.
    /// </summary>
    public class CollectableItem : CollectableBase, IDropable
    {
        [SerializeField]
        ItemBase m_Item;

        public ItemBase Item => m_Item;

        [SerializeField]
        GameObject m_Graphics;

        public override string Name => m_Item?.Name;

        public override bool TryPick()
        {
            if (IsCollected)
            {
                return false;
            }

            IsCollected = true;
            HideGraphics();
            return true;
        }

        public bool TryDrop(Vector3 location)
        {
            if (!IsCollected)
            {
                return false;
            }

            IsCollected = false;

            transform.position = location;
            ShowGraphics();
            
            return true;
        }

        void ShowGraphics()
        {
            m_Graphics.SetActive(true);
        }

        void HideGraphics()
        {
            m_Graphics.SetActive(false);
        }
    }
}