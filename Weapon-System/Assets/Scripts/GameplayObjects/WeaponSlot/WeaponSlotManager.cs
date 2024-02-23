using UnityEngine;
using Weapon_System.Utilities;

namespace Weapon_System.GameplayObjects.WeaponSlot
{
    /// <summary>
    /// Manages various weapon slots properties
    /// </summary>
    public class WeaponSlotManager : MonoBehaviour
    {
        public enum Slots
        {
            None,
            Slot1,
            Slot2
        }

        private const int NO_OF_SLOTS = 2;      // None is not counted as a slot

        [SerializeField]
        WeaponSlotEventChannelSO m_CurrentWeaponSlotChangedEvent = default;

        WeaponBase[] m_WeaponSlots;         // The weapon slots to store weapons

        private void Awake()
        {
            m_WeaponSlots = new WeaponBase[NO_OF_SLOTS];
        }

        private void Start()
        {
            m_CurrentWeaponSlotChangedEvent.OnEventRaised += OnCurrentWeaponSlotChanged;
        }

        private void OnDestroy()
        {
            m_CurrentWeaponSlotChangedEvent.OnEventRaised -= OnCurrentWeaponSlotChanged;
        }

        private void OnCurrentWeaponSlotChanged(Slots slot)
        {
            Debug.Log("Current weapon slot changed to: " + slot.ToString());
        }
    }
}