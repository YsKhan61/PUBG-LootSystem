using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the AttachmentItemUIs of Magazine attachments
    /// </summary>
    public class MagazineAttachmentUIMediator : AttachmentUIMediator
    {
        protected override bool IsWeaponCompatible(in WeaponDataSO weaponData)
        {
            return weaponData.AllowedMagazineAttachments.Length > 0;
        }
    }
}
