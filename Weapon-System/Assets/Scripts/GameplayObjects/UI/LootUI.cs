using UnityEngine;
using UnityEngine.Serialization;
using Weapon_System.GameplayObjects.ItemsSystem;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.UI
{
    /// <summary>
    /// It is the Loot UI of the player 
    /// It will be toggled on/off by the player.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class LootUI : MonoBehaviour
    {
        [Header("Listens to")]

        [SerializeField, Tooltip("The event that will toggle the inventory")]
        BoolEventChannelSO m_ToggleInventoryEvent;

        CanvasGroup m_CanvasGroup;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            m_ToggleInventoryEvent.OnEventRaised += ToggleUI;

            ToggleUI(false);
        }

        private void OnDestroy()
        {
            m_ToggleInventoryEvent.OnEventRaised -= ToggleUI;
        }

        private void ToggleUI(bool value)
        {
            m_CanvasGroup.alpha = value ? 1 : 0;
            m_CanvasGroup.blocksRaycasts = value;
            m_CanvasGroup.interactable = value;
        }
    }
}