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
    /// Type of the item UI that will be set to the ItemSlotUI
    /// // Each type has their own ItemSlotUI
    /// // Common types are stored in the inventory list.
    /// </summary>
    public enum ItemUIType
    {
        Common,
        Gun,
        MuzzleAttachment,
        ScopeAttachment,
        MagazineAttachment,
        StockAttachment,
        ForegripAttachment,
    }

    public abstract class ItemDataSO : ScriptableObject
    {
        [SerializeField]
        ItemType m_Type;
        public ItemType Type => m_Type;

        [SerializeField]
        ItemUIType m_UIType;
        public ItemUIType UIType => m_UIType;

        [SerializeField]
        Sprite m_IconSprite;
        public Sprite IconSprite => m_IconSprite;
    }
}