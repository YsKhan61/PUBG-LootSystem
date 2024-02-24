

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class GunItem : CommonItem
    {
        public override bool StoreInInventory(Inventory inventory)
        {
            inventory.AddGunToGunSlot(this);
            return true;
        }
    }

}
