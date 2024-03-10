using Codice.CM.Common;
using UnityEngine;
using UnityStandardAssets.Utility;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        FloatEventChannelSO m_MouseXInputEvent;

        [SerializeField]
        FloatEventChannelSO m_MouseYInputEvent;

        [SerializeField] private CharacterController m_CharacterController;
        [SerializeField] private FirstPersonController m_FirstPersonController;

        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        public bool UseFovKick => m_UseFovKick;

        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;


        private Camera m_Camera;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;


        void Start()
        {
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_MouseLook.Init(transform, m_Camera.transform);
        }

        void Update()
        {
            RotateView();

            UpdateCameraPosition(m_FirstPersonController.Speed);

            // m_MouseLook.UpdateCursorLock();
        }

        public void DoBobCycle()
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
        }

        public void DoFOVKickUp()
        {
            StartCoroutine(m_FovKick.FOVKickUp());
        }

        public void DoFOVKickDown()
        {
            StartCoroutine(m_FovKick.FOVKickDown());
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform, m_MouseXInputEvent.Value, m_MouseYInputEvent.Value);
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_FirstPersonController.IsWalking ? 1f : m_FirstPersonController.RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }
    }
}
