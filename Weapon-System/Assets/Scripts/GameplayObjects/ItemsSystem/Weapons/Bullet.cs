using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class Bullet : MonoBehaviour, ISpawnable
    {
        public string Name => gameObject.name;

        public GameObject GameObject => gameObject;

        [SerializeField]
        int m_PoolSize;
        public int PoolSize => m_PoolSize;


    }
}

