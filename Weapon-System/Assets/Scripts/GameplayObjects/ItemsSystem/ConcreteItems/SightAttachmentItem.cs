using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The sight item that can be attached to any ISightHolder
    /// </summary>
    public class SightAttachmentItem : InventoryItem, IWeaponAttachment
    {
        public SightAttachmentDataSO SightAttachmentData => m_ItemData as SightAttachmentDataSO;

        bool m_IsAttached = false;
        public bool IsAttached => m_IsAttached;

        public bool AimDownSight()
        {
            Debug.Log("Aiming down sight through " + Name + " with ADS Zoom value of " + SightAttachmentData.ADSZoomValue);
            return true;
        }

        public bool AttachToWeapon(GunItem gun)
        {
            if (gun.SightAttachment != this)
            {
                Debug.LogError("First sight need to be attached by ISightHolder, then this method can be called from ISightHolder only!");
                return false;
            }

            m_RootGO.transform.SetParent(gun.SightHolderTransform);
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
    }

}
