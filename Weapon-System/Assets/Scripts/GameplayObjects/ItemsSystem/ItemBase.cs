using UnityEngine;
using UnityEngine.Serialization;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public abstract class ItemBase : MonoBehaviour
    {
        ItemDataSO m_ItemData;

        public ItemDataSO ItemData => m_ItemData;
        public string Name => m_ItemData.ToString();
    }
}