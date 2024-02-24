using System.Collections.Generic;
using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class Inventory : MonoBehaviour
    {
        [Header("Broadcast to")]
        [SerializeField]
        ItemDataEventChannelSO m_OnItemAddedEvent;


        [SerializeField]        // SerializeField is used only for Debug purposes
        List<ItemBase> m_Items;

        [SerializeField]
        ItemGun[] m_ItemGuns;
        

        private void Start()
        {
            m_Items = new List<ItemBase>();
        }

        public void AddItem(ItemBase item)
        {
            m_Items.Add(item);
            m_OnItemAddedEvent.RaiseEvent(item.ItemData);
            Debug.Log(item.Name + " added to inventory!");
        }

        public void RemoveItem(ItemBase item)
        {
            m_Items.Remove(item);
            Debug.Log(item.Name + " removed from inventory!");
        }
    }
}
