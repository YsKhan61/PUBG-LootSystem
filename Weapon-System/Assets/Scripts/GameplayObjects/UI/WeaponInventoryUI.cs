using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    public class WeaponInventoryUI : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField]
        BoolEventChannelSO m_ToggleInventoryEvent;

        [Space(10)]

        [SerializeField]
        GameObject m_Panel;

        [SerializeField]
        TextMeshProUGUI m_WeaponSlot1Text;

        [SerializeField]
        TextMeshProUGUI m_WeaponSlot2Text;

        [SerializeField]
        TextMeshProUGUI m_WeaponInHandText;

        private void Start()
        {
            m_ToggleInventoryEvent.OnEventRaised += OnToggleInventory;
            ToggleInventoryUI(false);
        }

        private void OnDestroy()
        {
            m_ToggleInventoryEvent.OnEventRaised -= OnToggleInventory;
        
        }

        private void OnToggleInventory(bool value)
        {
            ToggleInventoryUI(value);
        }

        private void ToggleInventoryUI(bool value)
        {
            m_Panel.SetActive(value);
        }
    }
}