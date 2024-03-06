using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// Manages the AttachmentItemUIs of Stock attachments
    /// </summary>
    public class StockAttachmentUIMediator : AttachmentUIMediator
    {
        protected override bool IsWeaponCompatible(in WeaponDataSO weaponData)
        {
            return weaponData.AllowedStockAttachments.Length > 0;
        }
    }
}
