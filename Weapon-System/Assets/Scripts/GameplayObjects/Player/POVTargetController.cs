using UnityEngine;
using UnityEngine.TextCore.Text;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.Player
{
    /// <summary>
    /// The target that will be rotated using mouse look input.
    /// The cinemachine player look camera will have this target as follow at reference.
    /// </summary>
    public class POVTargetController : MonoBehaviour
    {
        [SerializeField]
        FloatEventChannelSO m_MouseXInputEvent;

        [SerializeField]
        FloatEventChannelSO m_MouseYInputEvent;

        [SerializeField] Transform m_TargetTransform;

        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;

        [HideInInspector] public float XAngle;
        [HideInInspector] public float YAngle;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;

        private void Start()
        {
            m_CharacterTargetRot = transform.localRotation;
            m_CameraTargetRot = m_TargetTransform.localRotation;
        }

        private void Update()
        {
            XAngle += (m_MouseYInputEvent.Value * XSensitivity);
            YAngle += (m_MouseXInputEvent.Value * YSensitivity);

            m_CharacterTargetRot *= Quaternion.Euler(0f, YAngle, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-XAngle, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                m_TargetTransform.localRotation = Quaternion.Slerp(m_TargetTransform.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                transform.localRotation = m_CharacterTargetRot;
                m_TargetTransform.localRotation = m_CameraTargetRot;
            }

            XAngle = 0;
            YAngle = 0;
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
