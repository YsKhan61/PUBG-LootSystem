using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The grip item that can be attached to specific WeaponItem
    /// </summary>
    public class GripAttachmentItem : InventoryItem, IWeaponAttachment
    {
        public GripAttachmentDataSO GripAttachmentData => m_ItemData as GripAttachmentDataSO;

        bool m_IsAttached = false;
        public bool IsAttached => m_IsAttached;

        public float GetRecoilReduction()
        {
            Debug.Log("Reducing recoil through " + Name + " with reduction value of " + GripAttachmentData.RecoilReductionValue);
            return GripAttachmentData.RecoilReductionValue;
        }

        public bool AttachToWeapon(WeaponItem gun)
        {
            if (gun.SightAttachment != this)
            {
                Debug.LogError("First sight need to be attached by ISightHolder, then this method can be called from ISightHolder only!");
                return false;
            }

            m_RootGO.transform.SetParent(gun.GripHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_RootGO.transform.localRotation = Quaternion.identity;

            ShowGraphics();

            m_IsAttached = true;
            return true;
        }

        public bool DetachFromWeapon()
        {
            m_RootGO.transform.SetParent(null);
            HideGraphics();

            m_IsAttached = false;
            return true;
        }

        public bool IsWeaponCompatible(WeaponDataSO weaponData)
        {
            foreach (var tag in weaponData.AllowedGripAttachments)
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
