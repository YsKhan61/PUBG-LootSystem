using UnityEngine;
using UnityEngine.Serialization;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The base class for all items
    /// It contains a name and the reference to the ItemDataSO
    /// </summary>
    public abstract class ItemBase : MonoBehaviour, IItemInfo
    {
        [SerializeField]
        protected ItemDataSO m_ItemData;
        public ItemDataSO ItemData => m_ItemData;
        public string Name => m_ItemData.name;
    }
}