using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Hand that is used to use the item
    /// </summary>
    public class HandToUseItem : MonoBehaviour
    {
        [Header("Listen to")]

        [SerializeField]
        BoolEventChannelSO m_FiringInput;

        public IUsable ItemInHand { get; set; }


        private void Update()
        {
            if (m_FiringInput.Value)
            {
                ItemInHand?.Use();
            }
        }
    }
}


