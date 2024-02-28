using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The sight item that can be attached to specific WeaponItem
    /// </summary>
    public class SightAttachmentItem : InventoryItem, IWeaponAttachment
    {
        public ItemTagSO ItemTag => m_ItemData.ItemTag;
        public SightAttachmentDataSO SightAttachmentData => m_ItemData as SightAttachmentDataSO;

        private WeaponItem m_WeaponItem;

        public bool AimDownSight()
        {
            Debug.Log("Aiming down sight through " + Name + " with ADS Zoom value of " + SightAttachmentData.ADSZoomValue);
            return true;
        }

        public bool AttachToWeapon(WeaponItem weapon)
        {
            if (m_WeaponItem != null)
            {
                Debug.LogError("Sight already attached: Sight need to be removed by UI Drag Drop");
                return false;
            }

            m_WeaponItem = weapon;
            m_WeaponItem.SightAttachment = this;

            m_RootGO.transform.SetParent(m_WeaponItem.SightHolderTransform);
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
            foreach (var tag in weaponData.AllowedSightAttachments)
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
