using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// This is the data of the gun item. It is added to the GunItem.
    /// Different guns such as M416, AKM, etc. will have different data.
    /// </summary>
    [CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/ItemDatas/GunDataSO", order = 0)]
    public class GunDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("How much will the player camera zoom in this iron sight ADS")]
        float m_ADSZoomValue;
        public float ADSZoomValue => m_ADSZoomValue;

        [SerializeField, Tooltip("The sight items allowed to be attached to this gun")]
        ItemTagSO[] m_AllowedSightAttachments;
        public ItemTagSO[] AllowedSightAttachments => m_AllowedSightAttachments;
    }
}