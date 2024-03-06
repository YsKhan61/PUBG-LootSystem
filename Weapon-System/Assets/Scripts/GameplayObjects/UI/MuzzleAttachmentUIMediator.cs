using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the AttachmentItemUIs of MuzzleAttachments
    /// </summary>
    public class MuzzleAttachmentUIMediator : AttachmentUIMediator
    {
        protected override bool IsWeaponCompatible(in WeaponDataSO weaponData)
        {
            return weaponData.AllowedMuzzleAttachments.Length > 0;
        }
    }
}
