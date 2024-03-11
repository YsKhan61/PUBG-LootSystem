using UnityEngine;
using UnityStandardAssets.Utility;
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

        [SerializeField] private MouseLook m_MouseLook;

        [SerializeField] Transform m_TargetTransform;

        private void Start()
        {
            m_MouseLook.Init(transform, m_TargetTransform);
        }

        private void Update()
        {
            m_MouseLook.LookRotation(transform, m_TargetTransform, m_MouseXInputEvent.Value * Time.deltaTime, m_MouseYInputEvent.Value * Time.deltaTime);
        }
    }
}
