using UnityEngine;
using Weapon_System.Utilities;

namespace Weapon_System.GameplayObjects.ItemsSystem.WeaponSystem
{
    /// <summary>
    /// Items that can damage targets.
    /// </summary>
    public abstract class WeaponBase : ItemBase
    {

        /*public enum WeaponType
        {
            AR,
            DMR,
            SMG,
            SR,
            Shotgun,
            Handgun,
            Melee,
            Throwable,
            Misselaneous
        }

        [SerializeField, Tooltip("Type of the weapon")]
        WeaponType m_WeaponType;

        [SerializeField, Tooltip("Damage done by the weapon")]
        int m_Damage;

        [SerializeField, Tooltip("The range upto which this weapon can register damage")]
        int m_Range;

        [SerializeField, Tooltip("Rate of firing per second")]
        float m_FireRate;

        [SerializeField, Tooltip("Force produced by firing from this weapon")]
        float m_Force;*/

        public abstract void Shoot();
    }
}
