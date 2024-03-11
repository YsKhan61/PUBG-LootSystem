using Cinemachine;
using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class AimCameraController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera m_ADSVirtualCamera;
        [SerializeField] CinemachineBrain m_CinemachineBrain;

        private void Start()
        {
            SetPriorityValue(0);
        }

        public void SetFOV(float fovValue)
        {
            m_ADSVirtualCamera.m_Lens.FieldOfView = fovValue;
        }

        public void SetTransitionTime(float seconds)
        {
            m_CinemachineBrain.m_DefaultBlend.m_Time = seconds;
        }

        public void SetPriorityValue(int value)
        {
            m_ADSVirtualCamera.Priority = value;
        }
    }

}
