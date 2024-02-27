using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "SightAttachmentData", menuName = "ScriptableObjects/ItemDatas/SightAttachmentDataSO")]
    public class SightAttachmentDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("How much will the player camera zoom in this iron sight ADS")]
        float m_ADSZoomValue;
        public float ADSZoomValue => m_ADSZoomValue;
    }

}