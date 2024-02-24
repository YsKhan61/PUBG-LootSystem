
using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public interface IName
    { 
        public string Name { get; }
    }

    public interface ICollectable : IName
    {
        public bool Collect();
    }

    public interface IDropable : IName
    {
        public bool Drop(Vector3 location);
    }

    public interface IStorable : IName
    {
        public bool StoreInInventory(Inventory inventory);
    }

    public interface  IUsable : IName
    {
        public bool Use();
    }
}