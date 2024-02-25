using System;
using System.Collections.Generic;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class Inventory : MonoBehaviour
    {
        [Header("Broadcast to")]

        [SerializeField]
        CommonItemEventChannelSO m_OnCommonItemAddedEvent;

        [SerializeField]
        GunItemEventChannelSO m_OnGunItemAddedEvent;

        [Header("Listens to")]

        [SerializeField]
        BoolEventChannelSO m_PrimaryWeaponSelectInputEvent;

        [SerializeField]
        BoolEventChannelSO m_SecondaryWeaponSelectInputEvent;

        [SerializeField]
        CommonItemEventChannelSO m_OnCommonItemRemovedEvent;

        [Space(10)]

        [SerializeField]
        ItemUserHand m_UserHand;

        [Space(10)]

        [Header("Common items")]

        [SerializeField]        // SerializeField is used only for Debug purposes
        List<CommonItem> m_CommonItems;

        [Space(10)]

        [Header("Gun items")]

        [SerializeField]        // SerializeField is used only for Debug purposes
        GunItem[] m_ItemGuns;

        private void Start()
        {
            m_CommonItems = new List<CommonItem>();
            m_ItemGuns = new GunItem[2];

            m_PrimaryWeaponSelectInputEvent.OnEventRaised += OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised += OnSecondaryWeaponSelect;
            m_OnCommonItemRemovedEvent.OnEventRaised += RemoveCommonItem;
        }

        private void OnDestroy()
        {
            m_PrimaryWeaponSelectInputEvent.OnEventRaised -= OnPrimaryWeaponSelect;
            m_SecondaryWeaponSelectInputEvent.OnEventRaised -= OnSecondaryWeaponSelect;
            m_OnCommonItemRemovedEvent.OnEventRaised -= RemoveCommonItem;
        }

        public void AddCommonItem(CommonItem item)
        {
            m_CommonItems.Add(item);
            m_OnCommonItemAddedEvent.RaiseEvent(item);
            Debug.Log(item.Name + " added to inventory!");
        }

        public void RemoveCommonItem(CommonItem item)
        {
            m_CommonItems.Remove(item);
            Debug.Log(item.Name + " removed from inventory!");
            item.Drop(transform.position + transform.forward * 2f);
        }

        public void AddGunToGunSlot(GunItem item)
        {
            // Add to the first empty slot
            for (int i = 0; i < m_ItemGuns.Length; i++)
            {
                if (m_ItemGuns[i] == null)
                {
                    m_ItemGuns[i] = item;
                    m_UserHand.ItemInHand = item;                  // Set the gun in hand
                    m_OnGunItemAddedEvent.RaiseEvent(item, i);
                    Debug.Log(item.Name + " added to inventory!");
                    return;
                }
            }

            // If no empty slot, replace the current gun
            for (int i = 0; i < m_ItemGuns.Length; i++)
            {
                if (m_ItemGuns[i] as IUsable == m_UserHand.ItemInHand) // (m_ItemGuns[i].IsInHand)
                {
                    m_ItemGuns[i] = item;
                    m_UserHand.ItemInHand = item;                  // Set the gun in hand
                    m_OnGunItemAddedEvent.RaiseEvent(item, i);
                    Debug.Log(item.Name + " added to inventory!");
                    return;
                }
            }

            // If no empty slot and no gun in hand, replace the first gun
            m_ItemGuns[0] = item;
            m_UserHand.ItemInHand = item;                  // Set the gun in hand
            m_OnGunItemAddedEvent.RaiseEvent(item, 0);
            Debug.Log(item.Name + " added to inventory!");
        }

        private void OnPrimaryWeaponSelect(bool _)
        {
            m_UserHand.ItemInHand = m_ItemGuns[0];
        }

        private void OnSecondaryWeaponSelect(bool _)
        {
            m_UserHand.ItemInHand = m_ItemGuns[1];
        }
    }
}
