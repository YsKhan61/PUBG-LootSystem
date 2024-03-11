using UnityEngine;



namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class WeaponAnimator : MonoBehaviour
    {
        const string AIM = "aim";

        [SerializeField]
        Animator m_Animator;


        public void AimIn()
        {
            if (!m_Animator.GetBool(AIM))
                m_Animator.SetBool(AIM, true);
        }

        public void AimOut()
        {
            if (m_Animator.GetBool(AIM))
                m_Animator.SetBool(AIM, false);
        }
    }
}


