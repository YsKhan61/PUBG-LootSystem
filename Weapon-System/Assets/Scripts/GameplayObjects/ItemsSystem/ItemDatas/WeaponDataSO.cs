using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// This is the data of the gun item. It is added to the GunItem.
    /// Different guns such as M416, AKM, etc. will have different data.
    /// </summary>
    [CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/ItemDatas/GunDataSO", order = 0)]
    public class WeaponDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("The prefab of the bullet")]
        Bullet m_BulletPrefab;
        public Bullet BulletPrefab => m_BulletPrefab;


        [SerializeField, Tooltip("How much will the player camera zoom in this iron sight ADS")]
        float m_ADSZoomValue;
        public float ADSZoomValue => m_ADSZoomValue;


        [SerializeField, Tooltip("No of bullet shots per second")]
        float m_FireRate;
        public float FireRate => m_FireRate;


        [SerializeField, Tooltip("The sight items allowed to be attached to this gun")]
        ItemTagSO[] m_AllowedSightAttachments;
        public ItemTagSO[] AllowedSightAttachments => m_AllowedSightAttachments;


        [SerializeField, Tooltip("The grip items allowed to be attached to this gun")]
        ItemTagSO[] m_AllowedGripAttachments;
        public ItemTagSO[] AllowedGripAttachments => m_AllowedGripAttachments;


        [SerializeField, Tooltip("The muzzle items allowed to be attached to this gun")]
        ItemTagSO[] m_AllowedMuzzleAttachments;
        public ItemTagSO[] AllowedMuzzleAttachments => m_AllowedMuzzleAttachments;


        [SerializeField, Tooltip("The magaize items allowed to be attached to this gun")]
        ItemTagSO[] m_AllowedMagazineAttachments;
        public ItemTagSO[] AllowedMagazineAttachments => m_AllowedMagazineAttachments;


        [SerializeField, Tooltip("The grip items allowed to be attached to this gun")]
        ItemTagSO[] m_AllowedStockAttachments;
        public ItemTagSO[] AllowedStockAttachments => m_AllowedStockAttachments;
    }
}