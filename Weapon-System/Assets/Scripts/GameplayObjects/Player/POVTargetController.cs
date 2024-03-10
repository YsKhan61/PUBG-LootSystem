using UnityEngine;
using UnityStandardAssets.Utility;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.Player
{
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
            RotateView();
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_TargetTransform, m_MouseXInputEvent.Value, m_MouseYInputEvent.Value);
        }
    }
}
