using Cinemachine;
using UnityEngine;


namespace Weapon_System.GameplayObjects.Player
{
    public class ADSController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera m_ADSVirtualCamera;

        public void StartADS(int fovValue)
        {
            m_ADSVirtualCamera.m_Lens.FieldOfView = fovValue;

        }
    }

}
