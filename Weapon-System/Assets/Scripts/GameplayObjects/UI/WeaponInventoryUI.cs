using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Weapon_System.GameplayObjects.UI
{
    public class WeaponInventoryUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_Panel;

        [SerializeField]
        TextMeshProUGUI m_WeaponSlot1Text;

        [SerializeField]
        TextMeshProUGUI m_WeaponSlot2Text;

        [SerializeField]
        TextMeshProUGUI m_WeaponInHandText;

        private void Start()
        {
            CloseInventory();
        }

        public void OpenInventory()
        {
            m_Panel.SetActive(true);
        }

        public void CloseInventory()
        {
            m_Panel.SetActive(false);
        }


    }
}