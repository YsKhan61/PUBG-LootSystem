using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// The sight item that can be attached to the gun
    /// </summary>
    public class SightAttachmentItem : InventoryItem, ISightAttachment
    {
        bool m_IsAttached = false;
        public bool IsAttached => m_IsAttached;

        public bool AimDownSight()
        {
            Debug.Log("Aiming down sight through " + Name);
            return true;
        }

        public bool AttachToWeapon(ISightHolder sightHolder)
        {
            if (sightHolder.SightAttachment != this as ISightAttachment)
            {
                Debug.LogError("First sight need to be attached by ISightHolder, then this method can be called from ISightHolder only!");
                return false;
            }

            m_RootGO.transform.SetParent(sightHolder.SightHolderTransform);
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
