
using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /*public interface IName
    { 
        public string Name { get; }
    }*/

    public interface ICollector
    {
        public Transform Transform { get; }
    }

    public interface ICollectable
    {
        public bool IsCollected { get; }
        public bool Collect(ICollector collector);
    }

    public interface IDropable
    {
        public bool Drop();
    }

    public interface IStorable
    {
        public bool StoreInInventory(Inventory inventory);
    }

    public interface  IUsable
    {
        public bool Use();
    }

    public interface IShooter
    {
        public bool Shoot();
    }
}