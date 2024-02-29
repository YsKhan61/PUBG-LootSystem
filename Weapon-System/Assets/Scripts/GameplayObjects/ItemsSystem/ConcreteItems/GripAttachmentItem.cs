using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The grip item that can be attached to specific WeaponItem
    /// </summary>
    public class GripAttachmentItem : InventoryItem, IWeaponAttachment
    {
        public GripAttachmentDataSO GripAttachmentData => m_ItemData as GripAttachmentDataSO;

        private WeaponItem m_WeaponItem;

        public float GetRecoilReduction()
        {
            Debug.Log("Reducing recoil through " + Name + " with reduction value of " + GripAttachmentData.RecoilReductionValue);
            return GripAttachmentData.RecoilReductionValue;
        }

        public bool AttachToWeapon(WeaponItem weapon)
        {
            m_WeaponItem = weapon;
            m_WeaponItem.GripAttachment = this;

            m_RootGO.transform.SetParent(m_WeaponItem.GripHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_RootGO.transform.localRotation = Quaternion.identity;

            ShowGraphics();

            return true;
        }

        public bool DetachFromWeapon()
        {
            if (m_WeaponItem == null)
            {
                Debug.LogError("Grip not attached: Grip need to be attached by UI Drag Drop");
                return false;
            }

            m_RootGO.transform.SetParent(null);

            HideGraphics();

            m_WeaponItem.GripAttachment = null;
            m_WeaponItem = null;

            return true;
        }

        public bool IsWeaponCompatible(WeaponDataSO weaponData)
        {
            foreach (var tag in weaponData.AllowedGripAttachments)
            {
                if (tag == GripAttachmentData.ItemTag)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
