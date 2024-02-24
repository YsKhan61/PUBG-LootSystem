using UnityEngine;
using UnityEngine.Serialization;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("m_ItemTag")]
        ItemDataSO m_ItemData;

        public ItemDataSO ItemData => m_ItemData;
        public string Name => m_ItemData.ToString();
    }
}