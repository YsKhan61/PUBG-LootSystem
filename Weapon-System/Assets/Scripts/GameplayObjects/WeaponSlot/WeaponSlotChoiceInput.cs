using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.WeaponSlot
{
    /// <summary>
    /// Choose the weapon slot to use using respective InputActionReferences
    /// </summary>
    public class WeaponSlotChoiceInput : MonoBehaviour
    {
        [SerializeField, Tooltip("The input action to choose slot 1")]
        InputActionReference m_Slot1ChooseInputActionReference;

        [SerializeField, Tooltip("The input action to choose slot 2")]
        InputActionReference m_Slot2ChooseInputActionReference;

        [SerializeField, Tooltip("The input action to Holster")]
        InputActionReference m_HolsterInputActionReference;

        [SerializeField]
        WeaponSlotEventChannelSO m_CurrentWeaponSlotChangedEvent = default;

        private void Awake()
        {
            SubscribeToAllInputActionReferences();
            EnableAllInputActionReferences();

            m_CurrentWeaponSlotChangedEvent.RaiseEvent(WeaponSlotManager.Slots.None);
        }

        private void OnDestroy()
        {
            DisableAllInputActionReferences();
            UnsubscribeFromAllInputActionReferences();
        }

        private void OnSlot1ChooseInputActionPerformed(InputAction.CallbackContext context)
        {
            ChooseSlot(WeaponSlotManager.Slots.Slot1);
        }

        private void OnSlot2ChooseInputActionPerformed(InputAction.CallbackContext context)
        {
            ChooseSlot(WeaponSlotManager.Slots.Slot2);
        }

        private void OnHolsterInputActionPerformed(InputAction.CallbackContext context)
        {
            ChooseSlot(WeaponSlotManager.Slots.None);
        }

        private void ChooseSlot(WeaponSlotManager.Slots slot)
        {
            m_CurrentWeaponSlotChangedEvent.RaiseEvent(slot);
        }

        private void SubscribeToAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.performed += OnSlot1ChooseInputActionPerformed;
            m_Slot2ChooseInputActionReference.action.performed += OnSlot2ChooseInputActionPerformed;
            m_HolsterInputActionReference.action.performed += OnHolsterInputActionPerformed;
        }

        private void UnsubscribeFromAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.performed -= OnSlot1ChooseInputActionPerformed;
            m_Slot2ChooseInputActionReference.action.performed -= OnSlot2ChooseInputActionPerformed;
            m_HolsterInputActionReference.action.performed -= OnHolsterInputActionPerformed;
        }

        private void EnableAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.Enable();
            m_Slot2ChooseInputActionReference.action.Enable();
            m_HolsterInputActionReference.action.Enable();
        }

        private void DisableAllInputActionReferences()
        {
            m_Slot1ChooseInputActionReference.action.Disable();
            m_Slot2ChooseInputActionReference.action.Disable();
            m_HolsterInputActionReference.action.Disable();
        }
    }

}
