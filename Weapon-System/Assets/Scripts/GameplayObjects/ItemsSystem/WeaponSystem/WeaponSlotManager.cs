using JetBrains.Annotations;
using System;
using UnityEngine;
using Weapon_System.Utilities;

namespace Weapon_System.GameplayObjects.WeaponSystem
{
    /// <summary>
    /// Stores weapons in a slot
    /// </summary>
    [Serializable]
    public class WeaponSlot
    {
        WeaponSlotManager.SlotLabel m_Label;
        public WeaponSlotManager.SlotLabel Label => m_Label;

        WeaponBase m_Weapon;
        public WeaponBase Weapon => m_Weapon;

        public WeaponSlot(WeaponSlotManager.SlotLabel label)
        {
            m_Label = label;
        }


        public void AddWeapon(WeaponBase weapon)
        {
            m_Weapon = weapon;
        }
    }

    /// <summary>
    /// Creates weapon slots as specified
    /// Selects a weapon slot to store weapons based on the input
    /// Interact with the weapon of the current slot
    /// </summary>
    public class WeaponSlotManager : MonoBehaviour
    {
        #region WeaponSlots Properties
        /// <summary>
        /// Datas related to all slots need to be stored in WeaponSlot Manager
        /// Datas related to a single slot need to be stored in WeaponSlot
        /// </summary>

        private const int NO_OF_SLOTS = 2;      // None is not counted as a slot

        public enum SlotLabel : int
        {
            Slot1 = 0,
            Slot2 = 1
        }

        #endregion

        [Header("Listens to")]
        [SerializeField]
        WeaponSlotEventChannelSO m_CurrentWeaponSlotChangedEvent = default;

        [SerializeField]
        BoolEventChannelSO m_HolserWeaponEvent = default;

        [SerializeField]
        BoolEventChannelSO m_FiringWeaponEvent = default;

        [Space(10)]
        

        [SerializeField]
        WeaponSlot[] weaponSlots;// The weapon slots to store weapons

        WeaponSlot m_CurrentWeaponSlot;
        public WeaponSlot CurrentWeaponSlot => m_CurrentWeaponSlot;

        private void Start()
        {
            weaponSlots = new WeaponSlot[NO_OF_SLOTS];
            for (int i = 0; i < NO_OF_SLOTS; i++)
            {
                weaponSlots[i] = new WeaponSlot((WeaponSlotManager.SlotLabel)i);
            }

            m_CurrentWeaponSlotChangedEvent.OnEventRaised += OnCurrentWeaponSlotChanged;
            m_HolserWeaponEvent.OnEventRaised += OnHolsterWeapon;

            m_CurrentWeaponSlot = weaponSlots[0];
        }

        private void OnDestroy()
        {
            m_CurrentWeaponSlotChangedEvent.OnEventRaised -= OnCurrentWeaponSlotChanged;
            m_HolserWeaponEvent.OnEventRaised -= OnHolsterWeapon;
        }

        private void Update()
        {
            if (m_CurrentWeaponSlot != null && 
                m_CurrentWeaponSlot.Weapon != null &&
                m_FiringWeaponEvent.Value)
            {
                m_CurrentWeaponSlot.Weapon.Shoot();
            }
        }

        public void AddWeaponToSlot(SlotLabel slotLabel, WeaponBase weapon)
        {
            if (weaponSlots[(int)slotLabel] == null)
            {
                Debug.LogError("Weapon slot not found: " + slotLabel.ToString());
                return;
            }

            foreach (WeaponSlot slot in weaponSlots)
            {
                if (slot.Weapon != null)
                {
                    continue;
                }
                // If empty slot found
                weaponSlots[(int)slotLabel].AddWeapon(weapon);
                return;
            }
            // If no empty slot found
            m_CurrentWeaponSlot.AddWeapon(weapon);
        }

        private void OnCurrentWeaponSlotChanged(WeaponSlotManager.SlotLabel label)
        {
            m_CurrentWeaponSlot = weaponSlots[(int)label];
            Debug.Log("Current weapon slot changed to: " + m_CurrentWeaponSlot.Label.ToString());
        }

        private void OnHolsterWeapon(bool _)
        {
            m_CurrentWeaponSlot = null;
            Debug.Log("Weapon holstered");
        }
    }
}