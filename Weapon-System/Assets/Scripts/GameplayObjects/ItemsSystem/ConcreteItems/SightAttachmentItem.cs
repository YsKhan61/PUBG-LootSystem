using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The sight item that can be attached to specific WeaponItem
    /// </summary>
    public class SightAttachmentItem : InventoryItem, IWeaponAttachment
    {
        public SightAttachmentDataSO SightAttachmentData => m_ItemData as SightAttachmentDataSO;

        private WeaponItem m_WeaponItem;

        public bool StartAimDownSight()
        {
            // Debug.Log("Aiming down sight through " + Name + " with ADS Zoom value of " + SightAttachmentData.ADS_FOV);
            
            m_WeaponItem.ItemUserHand.AimCameraController.SetPriorityValue(20);
            m_WeaponItem.WeaponAnimator.AimIn();

            return true;
        }

        public bool StopAimDownSight()
        {
            // Debug.Log("Stopped aiming down sight through " + Name);

            m_WeaponItem.ItemUserHand.AimCameraController.SetPriorityValue(0);
            m_WeaponItem.WeaponAnimator.AimOut();

            return true;
        }

        public bool AttachToWeapon(WeaponItem weapon)
        {
            SetReferences(weapon);
            SetParentToHolder();
            ShowGraphics();
            Configure();
            
            return true;
        }

        public bool DetachFromWeapon()
        {
            if (m_WeaponItem == null)
            {
                Debug.LogError("Sight not attached: Sight need to be attached by UI Drag Drop");
                return false;
            }

            ResetParent();
            HideGraphics();
            RemoveReferences();

            return true;
        }

        public bool IsWeaponCompatible(WeaponDataSO weaponData)
        {
            foreach (var tag in weaponData.AllowedSightAttachments)
            {
                if (tag == SightAttachmentData.ItemTag)
                {
                    return true;
                }
            }

            return false;
        }


        void SetReferences(in WeaponItem weapon)
        {
            m_WeaponItem = weapon;
            m_WeaponItem.SightAttachment = this;
        }

        void SetParentToHolder()
        {
            m_RootGO.transform.SetParent(m_WeaponItem.SightHolderTransform);
            m_RootGO.transform.localPosition = Vector3.zero;
            m_RootGO.transform.localRotation = Quaternion.identity;
        }

        void ResetParent()
        {
            m_RootGO.transform.SetParent(null);
        }

        void RemoveReferences()
        {
            m_WeaponItem.SightAttachment = null;
            m_WeaponItem = null;
        }

        void Configure()
        {
            m_WeaponItem.ItemUserHand.AimCameraController.SetFOV(SightAttachmentData.ADS_FOV);
            m_WeaponItem.ItemUserHand.AimCameraController.SetTransitionTime(SightAttachmentData.ADS_Time);
        }
    }

}
