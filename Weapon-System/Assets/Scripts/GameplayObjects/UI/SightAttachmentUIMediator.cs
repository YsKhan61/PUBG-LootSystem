
using UnityEngine;
using UnityEngine.Serialization;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the AttachmentItemUIs of SightAttachments
    /// </summary>
    public class SightAttachmentUIMediator : AttachmentUIMediator
    {
        protected override bool IsWeaponCompatible(in WeaponDataSO weaponData)
        {
            return weaponData.AllowedSightAttachments.Length > 0;
        }
    }
}
