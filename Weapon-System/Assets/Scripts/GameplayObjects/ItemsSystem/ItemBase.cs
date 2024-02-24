using UnityEngine;
using UnityEngine.Serialization;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The base class for all items
    /// </summary>
    public abstract class ItemBase : MonoBehaviour, IName
    {
        [SerializeField]
        protected ItemDataSO m_ItemData;
        public ItemDataSO ItemData => m_ItemData;
        public string Name => m_ItemData.name;
    }
}