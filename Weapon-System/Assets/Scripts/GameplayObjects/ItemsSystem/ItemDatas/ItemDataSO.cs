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
    }

    public abstract class ItemDataSO : ScriptableObject
    {
        [SerializeField]
        ItemType m_Type;
        public ItemType Type => m_Type;

        [SerializeField]
        Sprite m_Icon;
        public Sprite Icon => m_Icon;
    }
}