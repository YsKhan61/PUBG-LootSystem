using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public enum ItemUIType
    {
        Common,                 // These items can be stored in the Inventory Bag Panel
        Weapon,                    // These items can be stored in the Weapon Slots
        HandGun,                // These items can be stored in the HandGun Slots
        Throwables,             // These items can be stored in the Throwable Slots
        MuzzleAttachment,       // These items can be stored either in the Inventory Bag Panel or in the Muzzle Attachment Slots
        SightAttachment,        // These items can be stored either in the Inventory Bag Panel or in the Sight Attachment Slots
        MagazineAttachment,     // These items can be stored either in the Inventory Bag Panel or in the Magazine Attachment Slots
        StockAttachment,        // These items can be stored either in the Inventory Bag Panel or in the Stock Attachment Slots
        ForegripAttachment,     // These items can be stored either in the Inventory Bag Panel or in the Foregrip Attachment Slots
    }

    /// <summary>
    /// Data container of the Item that will not change at runtime.
    /// It will be same across all the instances of the item.
    /// </summary>
    public abstract class ItemDataSO : ScriptableObject
    {
        [SerializeField]
        ItemTagSO m_ItemTag;
        public ItemTagSO ItemTag => m_ItemTag;

        [SerializeField, Tooltip("The UI type of this item")]
        ItemUIType m_UIType;
        public ItemUIType UIType => m_UIType;

        [SerializeField, Tooltip("The UI icon of this item")]
        Sprite m_IconSprite;
        public Sprite IconSprite => m_IconSprite;
    }
}