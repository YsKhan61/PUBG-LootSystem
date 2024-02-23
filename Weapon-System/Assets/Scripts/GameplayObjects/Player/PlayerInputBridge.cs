using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.Player
{
    /// <summary>
    /// Bridges between InputActionReferences and their respective gameplay events
    /// </summary>
    public class PlayerInputBridge : MonoBehaviour
    {
        #region Broadcast EventChannels

        [Header("Broadcast to")]

        [Space(5)]

        [SerializeField]
        FloatEventChannelSO m_MouseXInputData;

        [SerializeField]
        FloatEventChannelSO m_MouseYInputData;

        [SerializeField]
        FloatEventChannelSO m_VerticalMoveInputData;

        [SerializeField]
        FloatEventChannelSO m_HorizontalMoveInputData;

        [SerializeField]
        BoolEventChannelSO m_JumpInputEvent;

        [SerializeField]
        BoolEventChannelSO m_RunInputEvent;

        [SerializeField]
        BoolEventChannelSO m_PrimaryWeaponSelectedEvent;

        [SerializeField]
        BoolEventChannelSO m_SecondaryWeaponSelectedEvent;

        [SerializeField]
        BoolEventChannelSO m_HolsterWeaponEvent = default;

        /// <summary>
        /// Firing input, Press -> True, Release -> False
        /// </summary>
        [SerializeField]
        BoolEventChannelSO m_FiringWeaponEvent = default;

        [Space(10)]

        #endregion

        #region Input Action References

        [Header("Input Action References")]

        [Space(5)]

        [SerializeField, Tooltip("Camera Look X")]
        InputActionReference m_MouseXInputActionReference;

        [SerializeField, Tooltip("Camera Look Y")]
        InputActionReference m_MouseYInputActionReference;

        [SerializeField, Tooltip("Move forward/backward")]
        InputActionReference m_VerticalInputActionReference;

        [SerializeField, Tooltip("Strafe left/right")]
        InputActionReference m_horizontalInputActionReference;

        [SerializeField, Tooltip("Jump")]
        InputActionReference m_JumpInputActionReference;

        [SerializeField, Tooltip("Run")]
        InputActionReference m_RunInputActionReference;

        [SerializeField, Tooltip("Fire weapon")]
        InputActionReference m_FireInputActionReference;

        [SerializeField, Tooltip("Reload weapon")]
        InputActionReference m_ReloadInputActionReference;

        [SerializeField, Tooltip("ADS")]
        InputActionReference m_ADSInputActionReference;


        [SerializeField, Tooltip("Choose slot 1")]
        InputActionReference m_PrimaryWeaponSelectInputActionReference;

        [SerializeField, Tooltip("Choose slot 2")]
        InputActionReference m_SecondaryWeaponSelectInputActionReference;

        [SerializeField, Tooltip("Holster")]
        InputActionReference m_HolsterInputActionReference;

        #endregion

        private void Awake()
        {
            SubscribeToAllInputActionReferences();
            EnableAllInputActionReferences();
        }

        private void Update()
        {
            m_MouseXInputData.SetValue(m_MouseXInputActionReference.action.ReadValue<float>());
            m_MouseYInputData.SetValue(m_MouseYInputActionReference.action.ReadValue<float>());

            m_VerticalMoveInputData.SetValue(m_VerticalInputActionReference.action.ReadValue<float>());
            m_HorizontalMoveInputData.SetValue(m_horizontalInputActionReference.action.ReadValue<float>());
        }

        private void OnDestroy()
        {
            DisableAllInputActionReferences();
            UnsubscribeFromAllInputActionReferences();
        }

        private void OnRunInputActionStarted(InputAction.CallbackContext context)
        {
            m_RunInputEvent?.SetValue(true);
        }

        private void OnRunInputActionEnded(InputAction.CallbackContext context)
        {
            m_RunInputEvent?.SetValue(false);
        }

        private void OnJumpInputActionPerformed(InputAction.CallbackContext context)
        {
            m_JumpInputEvent?.SetValue(true);
        }

        private void OnPrimaryWeaponSelectActionPerformed(InputAction.CallbackContext context)
        {
            m_PrimaryWeaponSelectedEvent?.RaiseEvent();
        }

        private void OnSecondaryWeaponSelectActionPerformed(InputAction.CallbackContext context)
        {
            m_SecondaryWeaponSelectedEvent?.RaiseEvent();
        }

        private void OnHolsterInputActionPerformed(InputAction.CallbackContext context)
        {
            m_HolsterWeaponEvent.RaiseEvent();
        }

        private void OnFireInputActionStarted(InputAction.CallbackContext context)
        {
            m_FiringWeaponEvent.SetValueAndRaiseEvent(true);
        }

        private void OnFireInputActionCanceled(InputAction.CallbackContext context)
        {
            m_FiringWeaponEvent.SetValueAndRaiseEvent(false);
        }

        private void SubscribeToAllInputActionReferences()
        {
            m_RunInputActionReference.action.started += OnRunInputActionStarted;
            m_RunInputActionReference.action.canceled += OnRunInputActionEnded;
            m_JumpInputActionReference.action.performed += OnJumpInputActionPerformed;

            m_PrimaryWeaponSelectInputActionReference.action.performed += OnPrimaryWeaponSelectActionPerformed;
            m_SecondaryWeaponSelectInputActionReference.action.performed += OnSecondaryWeaponSelectActionPerformed;
            m_HolsterInputActionReference.action.performed += OnHolsterInputActionPerformed;

            m_FireInputActionReference.action.started += OnFireInputActionStarted;
            m_FireInputActionReference.action.canceled += OnFireInputActionCanceled;
        }

        private void UnsubscribeFromAllInputActionReferences()
        {
            m_RunInputActionReference.action.performed -= OnRunInputActionStarted;
            m_RunInputActionReference.action.canceled -= OnRunInputActionEnded;
            m_JumpInputActionReference.action.performed -= OnJumpInputActionPerformed;

            m_PrimaryWeaponSelectInputActionReference.action.performed -= OnPrimaryWeaponSelectActionPerformed;
            m_SecondaryWeaponSelectInputActionReference.action.performed -= OnSecondaryWeaponSelectActionPerformed;
            m_HolsterInputActionReference.action.performed -= OnHolsterInputActionPerformed;

            m_FireInputActionReference.action.started -= OnFireInputActionStarted;
            m_FireInputActionReference.action.canceled -= OnFireInputActionCanceled;
        }

        private void EnableAllInputActionReferences()
        {
            m_MouseXInputActionReference.action.Enable();
            m_MouseYInputActionReference.action.Enable();

            m_VerticalInputActionReference.action.Enable();
            m_horizontalInputActionReference.action.Enable();
            m_JumpInputActionReference.action.Enable();
            m_RunInputActionReference.action.Enable();

            m_PrimaryWeaponSelectInputActionReference.action.Enable();
            m_SecondaryWeaponSelectInputActionReference.action.Enable();
            m_HolsterInputActionReference.action.Enable();
            m_FireInputActionReference.action.Enable();
        }

        private void DisableAllInputActionReferences()
        {
            m_MouseXInputActionReference.action.Disable();
            m_MouseYInputActionReference.action.Disable();

            m_VerticalInputActionReference.action.Disable();
            m_horizontalInputActionReference.action.Disable();
            m_JumpInputActionReference.action.Disable();
            m_RunInputActionReference.action.Disable();

            m_PrimaryWeaponSelectInputActionReference.action.Disable();
            m_SecondaryWeaponSelectInputActionReference.action.Disable();
            m_HolsterInputActionReference.action.Disable();
            m_FireInputActionReference.action.Disable();
        }
    }
}
