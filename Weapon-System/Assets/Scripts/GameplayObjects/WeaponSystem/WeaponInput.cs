using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.WeaponSystem
{
    /// <summary>
    /// Inputs such as shoot, reload, ADS, choose slot 1, choose slot 2, holster
    /// </summary>
    public class WeaponInput : MonoBehaviour
    {
        [Header("Broadcast to")]
        [SerializeField]
        WeaponSlotEventChannelSO m_CurrentWeaponSlotChangedEvent = default;

        [SerializeField]
        BoolEventChannelSO m_HolsterWeaponEvent = default;

        /// <summary>
        /// Firing input, Press -> True, Release -> False
        /// </summary>
        [SerializeField]
        BoolEventChannelSO m_FiringWeaponEvent = default;

        [Space(10)]

        [Header("Input Action References")]

        [SerializeField, Tooltip("Fire weapon")]
        InputActionReference m_FireInputActionReference;

        [SerializeField, Tooltip("Reload weapon")]
        InputActionReference m_ReloadInputActionReference;

        [SerializeField, Tooltip("ADS")]
        InputActionReference m_ADSInputActionReference;


        [SerializeField, Tooltip("Choose slot 1")]
        InputActionReference m_Slot1ChooseInputActionReference;

        [SerializeField, Tooltip("Choose slot 2")]
        InputActionReference m_Slot2ChooseInputActionReference;

        [SerializeField, Tooltip("Holster")]
        InputActionReference m_HolsterInputActionReference;

        private void Awake()
        {
            SubscribeToAllInputActionReferences();
            EnableAllInputActionReferences();
        }

        private void OnDestroy()
        {
            DisableAllInputActionReferences();
            UnsubscribeFromAllInputActionReferences();
        }

        private void OnSlot1ChooseInputActionPerformed(InputAction.CallbackContext context)
        {
            ChooseSlot(WeaponSlotManager.SlotLabel.Slot1);
        }

        private void OnSlot2ChooseInputActionPerformed(InputAction.CallbackContext context)
        {
            ChooseSlot(WeaponSlotManager.SlotLabel.Slot2);
        }

        private void OnHolsterInputActionPerformed(InputAction.CallbackContext context)
        {
            HideWeapon();
        }

        private void OnFireInputActionStarted(InputAction.CallbackContext context)
        {
            m_FiringWeaponEvent.RaiseEvent(true);
        }

        private void OnFireInputActionCanceled(InputAction.CallbackContext context)
        {
            m_FiringWeaponEvent.RaiseEvent(false);
        }

        private void ChooseSlot(WeaponSlotManager.SlotLabel slot)
        {
            m_CurrentWeaponSlotChangedEvent.RaiseEvent(slot);
        }

        private void HideWeapon()
        {
            m_HolsterWeaponEvent.RaiseEvent();
        }

        private void SubscribeToAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.performed += OnSlot1ChooseInputActionPerformed;
            m_Slot2ChooseInputActionReference.action.performed += OnSlot2ChooseInputActionPerformed;
            m_HolsterInputActionReference.action.performed += OnHolsterInputActionPerformed;

            m_FireInputActionReference.action.started += OnFireInputActionStarted;
            m_FireInputActionReference.action.canceled += OnFireInputActionCanceled;
        }

        private void UnsubscribeFromAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.performed -= OnSlot1ChooseInputActionPerformed;
            m_Slot2ChooseInputActionReference.action.performed -= OnSlot2ChooseInputActionPerformed;
            m_HolsterInputActionReference.action.performed -= OnHolsterInputActionPerformed;

            m_FireInputActionReference.action.started -= OnFireInputActionStarted;
            m_FireInputActionReference.action.canceled -= OnFireInputActionCanceled;
        }

        private void EnableAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.Enable();
            m_Slot2ChooseInputActionReference.action.Enable();
            m_HolsterInputActionReference.action.Enable();
            m_FireInputActionReference.action.Enable();
        }

        private void DisableAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.Disable();
            m_Slot2ChooseInputActionReference.action.Disable();
            m_HolsterInputActionReference.action.Disable();
            m_FireInputActionReference.action.Disable();
        }
    }

}
