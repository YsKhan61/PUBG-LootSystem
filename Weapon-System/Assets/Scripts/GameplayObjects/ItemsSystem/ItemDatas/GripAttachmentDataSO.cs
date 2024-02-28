using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Data related to various grip attachment items
    /// eg: Vertical Grip, Angled Grip etc.
    /// </summary>
    [CreateAssetMenu(fileName = "GripAttachmentData", menuName = "ScriptableObjects/ItemDatas/GripAttachmentDataSO")]
    public class GripAttachmentDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("Amount by which the recoil of the weapon will be reduced")]
        float m_RecoilReductionValue;
        public float RecoilReductionValue => m_RecoilReductionValue;
    }

}