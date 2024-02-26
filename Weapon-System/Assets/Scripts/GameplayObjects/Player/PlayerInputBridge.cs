using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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

        [SerializeField, FormerlySerializedAs("m_HolsterWeaponEvent")]
        BoolEventChannelSO m_HolsterItemEvent;

        /// <summary>
        /// Primary use case - Use, Fire, etc. , Press -> True, Release -> False
        /// </summary>
        [SerializeField]
        BoolEventChannelSO m_PrimaryUseInputEvent;

        /// <summary>
        /// Secondary use case - ADS, Press -> True, Release -> False
        /// </summary>
        [SerializeField]
        BoolEventChannelSO m_SecondaryUseInputEvent;

        [SerializeField]
        BoolEventChannelSO m_PickupItemEvent;

        [SerializeField]
        BoolEventChannelSO m_ToggleInventoryEvent;

        [Space(10)]

        #endregion


        [Space(10)]

        InputControls m_InputControls;



        private void Awake()
        {
            m_InputControls = new InputControls();

            SubscribeToAllInputActionReferences();
            EnableAllInputActionReferences();
        }

        // Locking cursor in the center of the game view at start of the game
        // Later need to shift these pre-start configs to a separate script
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            m_MouseXInputData.SetValue(m_InputControls.Player.MouseXAxis.ReadValue<float>());
            m_MouseYInputData.SetValue(m_InputControls.Player.MouseYAxis.ReadValue<float>());

            m_VerticalMoveInputData.SetValue(m_InputControls.Player.VerticalMove.ReadValue<float>());
            m_HorizontalMoveInputData.SetValue(m_InputControls.Player.HorizontalMove.ReadValue<float>());
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
            m_JumpInputEvent?.RaiseEvent();
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
            m_HolsterItemEvent.RaiseEvent();
        }

        private void OnPrimaryUseInputActionStarted(InputAction.CallbackContext context)
        {
            m_PrimaryUseInputEvent.SetValueAndRaiseEvent(true);
        }

        private void OnPrimaryUseInputActionCanceled(InputAction.CallbackContext context)
        {
            m_PrimaryUseInputEvent.SetValueAndRaiseEvent(false);
        }

        private void OnSecondaryUseInputActionStarted(InputAction.CallbackContext context)
        {
            m_SecondaryUseInputEvent.SetValueAndRaiseEvent(true);
        }

        private void OnSecondaryUseInputActionCanceled(InputAction.CallbackContext context)
        {
            m_SecondaryUseInputEvent.SetValueAndRaiseEvent(false);
        }

        private void OnPickupInputActionPerformed(InputAction.CallbackContext context)
        {
            m_PickupItemEvent.RaiseEvent();
        }

        private void OnToggleInventoryActionStarted(InputAction.CallbackContext obj)
        {
            m_ToggleInventoryEvent.SetValueAndRaiseEvent(!m_ToggleInventoryEvent.Value);

            if (m_ToggleInventoryEvent.Value)
            {
                m_InputControls.Player.Disable();
                m_InputControls.Player.VerticalMove.Enable();
                m_InputControls.Player.HorizontalMove.Enable();

                // enable mouse pointer
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                m_InputControls.Player.Enable();

                // disable mouse pointer
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }




        private void SubscribeToAllInputActionReferences()
        {
            m_InputControls.Player.Run.started += OnRunInputActionStarted;
            m_InputControls.Player.Run.canceled += OnRunInputActionEnded;

            m_InputControls.Player.Jump.performed += OnJumpInputActionPerformed;

            m_InputControls.Player.PrimaryWeapon_Select.performed += OnPrimaryWeaponSelectActionPerformed;

            m_InputControls.Player.SecondaryWeapon_Select.performed += OnSecondaryWeaponSelectActionPerformed;

            m_InputControls.Player.Holster.performed += OnHolsterInputActionPerformed;

            m_InputControls.Player.PrimaryUse.started += OnPrimaryUseInputActionStarted;
            m_InputControls.Player.PrimaryUse.canceled += OnPrimaryUseInputActionCanceled;

            m_InputControls.Player.SecondaryUse.started += OnSecondaryUseInputActionStarted;
            m_InputControls.Player.SecondaryUse.canceled += OnSecondaryUseInputActionCanceled;

            m_InputControls.Player.Pickup.performed += OnPickupInputActionPerformed;

            m_InputControls.Global.Toggle_Inventory.started += OnToggleInventoryActionStarted;
        }

        private void UnsubscribeFromAllInputActionReferences()
        { 
            m_InputControls.Player.Run.started -= OnRunInputActionStarted;
            m_InputControls.Player.Run.canceled -= OnRunInputActionEnded;

            m_InputControls.Player.Jump.performed -= OnJumpInputActionPerformed;

            m_InputControls.Player.PrimaryWeapon_Select.performed -= OnPrimaryWeaponSelectActionPerformed;

            m_InputControls.Player.SecondaryWeapon_Select.performed -= OnSecondaryWeaponSelectActionPerformed;

            m_InputControls.Player.Holster.performed -= OnHolsterInputActionPerformed;

            m_InputControls.Player.PrimaryUse.started -= OnPrimaryUseInputActionStarted;
            m_InputControls.Player.PrimaryUse.canceled -= OnPrimaryUseInputActionCanceled;

            m_InputControls.Player.SecondaryUse.started -= OnSecondaryUseInputActionStarted;
            m_InputControls.Player.SecondaryUse.canceled -= OnSecondaryUseInputActionCanceled;

            m_InputControls.Player.Pickup.performed -= OnPickupInputActionPerformed;

            m_InputControls.Global.Toggle_Inventory.started -= OnToggleInventoryActionStarted;
        }

        private void EnableAllInputActionReferences()
        {
            m_InputControls.Player.Enable();
            m_InputControls.Global.Enable();
        }

        private void DisableAllInputActionReferences()
        {
            m_InputControls.Player.Disable();
            m_InputControls.Global.Disable();
        }
    }
}
