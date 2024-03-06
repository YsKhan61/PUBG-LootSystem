using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Data container of the Item that will not change at runtime.
    /// It will be same across all the instances of the item.
    /// </summary>
    public abstract class ItemDataSO : ScriptableObject
    {
        [SerializeField]
        ItemTagSO m_ItemTag;
        public ItemTagSO ItemTag => m_ItemTag;

        [SerializeField, Tooltip("The UI tag of this item")]
        ItemUITagSO m_UITag;
        public ItemUITagSO UITag => m_UITag;

        [SerializeField, Tooltip("The UI icon of this item")]
        Sprite m_IconSprite;
        public Sprite IconSprite => m_IconSprite;

        [SerializeField, Tooltip("The space required for this item to be in inventory. Positive - Deduct space from inventory, Negative - Add space to inventory(items), Zero - Non space items")]
        int m_SpaceRequired;
        public int SpaceRequired => m_SpaceRequired;
    }
}