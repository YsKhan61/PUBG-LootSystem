using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Data related to various grip attachment items
    /// eg: Vertical Grip, Angled Grip etc.
    /// </summary>
    [CreateAssetMenu(fileName = "MuzzleAttachmentData", menuName = "ScriptableObjects/ItemDatas/MuzzleAttachmentDataSO")]
    public class MuzzleAttachmentDataSO : ItemDataSO
    {
        [SerializeField, Tooltip("Amount by which the recoil of the weapon will be reduced")]
        float m_RecoilReductionValue;
        public float RecoilReductionValue => m_RecoilReductionValue;

        [SerializeField, Tooltip("Amount by which the weapon's muzzle flash will be reduced")]
        float m_MuzzleFlashReductionValue;
        public float MuzzleFlashReductionValue => m_MuzzleFlashReductionValue;

        [SerializeField, Tooltip("Amount by which the weapon's sound will be reduced")]
        float m_SoundReductionValue;
        public float SoundReductionValue => m_SoundReductionValue;
    }

}