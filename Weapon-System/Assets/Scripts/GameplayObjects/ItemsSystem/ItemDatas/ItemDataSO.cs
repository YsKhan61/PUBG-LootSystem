using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public enum ItemType
    {
        Gun,
        Ammo,
        Armor,
        Heal,
        Throwables,
        GunAttachment,
    }

    /// <summary>
    /// Type of the item UI that will be set to the ItemSlotUI or ItemUI where other ItemUI can be dropped.
    /// </summary>
    public enum ItemUIType
    {
        Common,
        Gun,
        MuzzleAttachment,
        SightAttachment,
        MagazineAttachment,
        StockAttachment,
        ForegripAttachment,
    }

    /// <summary>
    /// Data container of the Item that will not change at runtime.
    /// It will be same across all the instances of the item.
    /// </summary>
    public abstract class ItemDataSO : ScriptableObject
    {
        [SerializeField, Tooltip("The type of this item")]
        ItemType m_Type;
        public ItemType Type => m_Type;

        [SerializeField, Tooltip("The UI type of this item")]
        ItemUIType m_UIType;
        public ItemUIType UIType => m_UIType;

        [SerializeField, Tooltip("The UI icon of this item")]
        Sprite m_IconSprite;
        public Sprite IconSprite => m_IconSprite;
    }
}