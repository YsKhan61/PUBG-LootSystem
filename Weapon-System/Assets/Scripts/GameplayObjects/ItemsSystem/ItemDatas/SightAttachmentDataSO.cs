using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Data related to various sight attachment items
    /// eg: Holographic, NightVisionScope, RedDot, 2xScope, 4xScope, 6xScope etc.
    /// </summary>
    [CreateAssetMenu(fileName = "SightAttachmentData", menuName = "ScriptableObjects/ItemDatas/SightAttachmentDataSO")]
    public class SightAttachmentDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("How much will the player camera zoom in this iron sight ADS")]
        float m_ADS_FOV;
        public float ADS_FOV => m_ADS_FOV;

        [SerializeField, Tooltip("The seconds needed to ads in, or ads out")]
        float m_ADS_Time;
        public float ADS_Time => m_ADS_Time;
    }

}