using Cinemachine;
using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class ADSController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera m_ADSVirtualCamera;
        [SerializeField] CinemachineBrain m_CinemachineBrain;

        private void Start()
        {
            StopADS();
        }

        public void SetADS(float fovValue)
        {
            m_ADSVirtualCamera.m_Lens.FieldOfView = fovValue;
        }

        public void SetTransitionTime(float seconds)
        {
            m_CinemachineBrain.m_DefaultBlend.m_Time = seconds;
        }

        public void StartADS()
        {
            m_ADSVirtualCamera.Priority = 20;
        }

        public void StopADS()
        {
            m_ADSVirtualCamera.Priority = 0;
        }
    }

}
