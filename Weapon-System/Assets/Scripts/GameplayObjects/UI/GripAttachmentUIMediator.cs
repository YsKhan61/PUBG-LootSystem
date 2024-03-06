using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the AttachmentItemUIs of Grip attachments
    /// </summary>
    public class GripAttachmentUIMediator : AttachmentUIMediator
    {
        protected override bool IsWeaponCompatible(in WeaponDataSO weaponData)
        {
            return weaponData.AllowedGripAttachments.Length > 0;
        }
    }
}
