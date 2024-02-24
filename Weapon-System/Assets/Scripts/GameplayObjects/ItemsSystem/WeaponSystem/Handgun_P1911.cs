using UnityEngine;
using Weapon_System;


namespace Weapon_System.GameplayObjects.ItemsSystem.WeaponSystem
{
    public class Handgun_P1911 : WeaponBase
    {
        public override void Shoot()
        {
            Debug.Log("P1911: Shooting...");
        }
    }
}

