using UnityEngine;
using Weapon_System.GameplayObjects.Player;


namespace Wepaon_System.GameplayObjects.ItemsSystem
{
    public class Recoil : MonoBehaviour
    {
        [SerializeField]
        Transform m_RecoilTransform;

        [SerializeField]
        POVTargetController m_PovTargetController;

        // Hipfire recoil
        [SerializeField]
        float m_HipfireRecoilX;
        [SerializeField]
        float m_HipfireRecoilY;
        [SerializeField]
        float m_HipfireRecoilZ;

        // Aimed recoil
        [SerializeField]
        float m_aimedRecoilX;
        [SerializeField]
        float m_aimedRecoilY;
        [SerializeField]
        float m_aimedRecoilZ;

        // Settings
        [SerializeField]
        float m_RecoilDuration;
        [SerializeField]
        float m_ResetDuration;

        float m_CurrentRecoilX;
        float m_CurrentRecoilY;
        float m_HorizontalRecoil;


        bool m_IsRecoiling = false;
        bool m_IsReseting = false;

        float m_Duration = 0;

        private void Update()
        {
            if (m_IsRecoiling)
            {
                if (m_Duration < m_RecoilDuration)
                {
                    m_Duration += Time.deltaTime;
                    ApplyVerticalRecoil();
                    ApplyHorizontalRecoil();
                }
                else
                {
                    m_IsRecoiling = false;
                    m_Duration = 0;
                    m_IsReseting = true;
                }
            }
            else if (m_IsReseting)
            {
                if (m_Duration < m_ResetDuration)
                {
                    m_Duration += Time.deltaTime;
                    ResetRecoil();
                }
                else
                {
                    m_IsReseting = false;
                    m_Duration = 0;
                }
            }
        }

        public void StartHipfireRecoil()
        {
            m_IsRecoiling = true;
            m_Duration = 0;

            m_CurrentRecoilX = m_HipfireRecoilX;
            m_CurrentRecoilY = Random.Range(-m_HipfireRecoilY, m_HipfireRecoilY);
        }

        public void StopResetRecoil()
        {
            m_IsReseting = false;
            m_Duration = 0;
        }

        void ApplyVerticalRecoil()
        {
            m_PovTargetController.XAngle = Mathf.Lerp(0f, m_CurrentRecoilX, (m_CurrentRecoilX/m_RecoilDuration) * Time.deltaTime);
        }

        void ApplyHorizontalRecoil()
        {
            m_HorizontalRecoil = m_CurrentRecoilY * (m_Duration / m_RecoilDuration);
            m_PovTargetController.YAngle = Mathf.Lerp(0f, m_HorizontalRecoil, (Mathf.Abs(m_CurrentRecoilY) / m_RecoilDuration) * Time.deltaTime);
        }

        void ResetRecoil()
        {
            m_PovTargetController.XAngle = Mathf.Lerp(0, -m_CurrentRecoilX, (m_CurrentRecoilX/m_ResetDuration) * Time.deltaTime);
            m_PovTargetController.YAngle = Mathf.Lerp(0, -m_HorizontalRecoil, (Mathf.Abs(m_CurrentRecoilY) / m_ResetDuration) * Time.deltaTime);
        }
    }

}
