using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon_System;
using Weapon_System.GameplayObjects.WeaponSystem;

namespace Weapon_System.Test
{
    /// <summary>
    /// Test weapon pickup and add to Weapon Slot
    /// </summary>
    public class WeaponFactoryTest : MonoBehaviour
    {
        [SerializeField]
        WeaponSlotManager m_WeaponSlotManager;

        // Start is called before the first frame update
        void Start()
        {
            WeaponBase p1911Weapon = new Handgun_P1911();
            WeaponBase m416Weapon = new AR_M416();
        }

        private void PickupWeapon()
        {
            m_WeaponSlotManager.AddWeaponToSlot(WeaponSlotManager.SlotLabel.Slot1, new Handgun_P1911());
        }
    }

}