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


        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
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
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform, m_MouseXInputEvent.Value, m_MouseYInputEvent.Value);
        }
    }
}
