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
        float m_ADSZoomValue;
        public float ADSZoomValue => m_ADSZoomValue;
    }

}