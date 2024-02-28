using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The sight item that can be attached to specific WeaponItem
    /// </summary>
    public class MuzzleAttachmentItem : InventoryItem, IWeaponAttachment
    {
        public MuzzleAttachmentDataSO MuzzleAttachmentData => m_ItemData as MuzzleAttachmentDataSO;

        private WeaponItem m_WeaponItem;

        public void GetRecoilReduction()
        {
            Debug.Log("Reducing recoil through " + Name + " with reduction value of " + MuzzleAttachmentData.RecoilReductionValue);
        }

        public void GetMuzzleFlashReduction()
        {
            Debug.Log("Reducing muzzle flash through " + Name + " with reduction value of " + MuzzleAttachmentData.MuzzleFlashReductionValue);
        }

        public void GetSoundReduction()
        {
            Debug.Log("Reducing sound through " + Name + " with reduction value of " + MuzzleAttachmentData.SoundReductionValue);
        }

        public bool AttachToWeapon(WeaponItem weapon)
        {
            m_WeaponItem = weapon;
            m_WeaponItem.MuzzleAttachment = this;

            m_RootGO.transform.SetParent(m_WeaponItem.MuzzleHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_RootGO.transform.localRotation = Quaternion.identity;

            ShowGraphics();

            return true;
        }

        public bool DetachFromWeapon()
        {
            if (m_WeaponItem == null)
            {
                Debug.LogError("Sight not attached: Sight need to be attached by UI Drag Drop");
                return false;
            }

            m_RootGO.transform.SetParent(null);

            HideGraphics();

            m_WeaponItem.SightAttachment = null;
            m_WeaponItem = null;

            return true;
        }

        public bool IsWeaponCompatible(WeaponDataSO weaponData)
        {
            foreach (var tag in weaponData.AllowedMuzzleAttachments)
            {
                if (tag == m_ItemData.ItemTag)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
