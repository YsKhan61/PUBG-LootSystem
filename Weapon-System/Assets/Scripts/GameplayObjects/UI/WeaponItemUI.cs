using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class WeaponItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField]
        Image m_Icon;

        [SerializeField]
        TextMeshProUGUI m_NameText;

        /*[SerializeField, Tooltip("The game object that will be toggled on/off")]
        GameObject m_PanelGO;*/

        public ItemDataSO ItemData { get; private set; }

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Vector2 m_lastAnchoredPosition;

        private WeaponInventoryUI m_InventoryUI;
        private GunItem m_Item;
        public GunItem Item => m_Item;

        [HideInInspector]
        public bool IsDragSuccess;

        private void Awake()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null)
            {
                Debug.LogError("No Canvas found in parent of " + gameObject.name);
                enabled = false;
                return;
            }

            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            Hide();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                m_InventoryUI.RemoveGunItemUIFromWeaponInventoryUI(m_Item);
                Hide();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_lastAnchoredPosition = m_RectTransform.anchoredPosition;
            IsDragSuccess = false;

            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_RectTransform.anchoredPosition += eventData.delta * m_Canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.alpha = 1f;

            if (!IsDragSuccess)
            {
                m_RectTransform.anchoredPosition = m_lastAnchoredPosition;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                if (m_InventoryUI != null)
                {
                    m_InventoryUI.GetWeaponSlotUI(this).TryDropItem(eventData.pointerDrag);
                }
            }
        }

        public void SetItemData(GunItem item, WeaponInventoryUI inventoryUI)
        {
            m_InventoryUI = inventoryUI;
            m_Item = item;
            ItemData = item.ItemData;
            m_Icon.sprite = ItemData.IconSprite;
            m_NameText.text = ItemData.name;

            Show();
        }

        private void Show()
        {
            m_CanvasGroup.alpha = 1;
            m_CanvasGroup.blocksRaycasts = true;
        }

        private void Hide()
        {
            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.blocksRaycasts = false;
        }
    }
}
