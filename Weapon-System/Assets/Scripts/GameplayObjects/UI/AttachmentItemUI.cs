using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class AttachmentItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField]
        Image m_Icon;

        [SerializeField]
        AttachmentUIMediator m_Mediator;
        public AttachmentUIMediator Mediator => m_Mediator;

        [SerializeField]
        int m_SlotIndex;
        public int SlotIndex => m_SlotIndex;

        private IWeaponAttachment m_StoredItem;
        public IWeaponAttachment StoredItem => m_StoredItem;

        [HideInInspector]
        public bool IsDragSuccess;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Transform m_LastParent;
        public Transform LastParent => m_LastParent;

        private Vector2 m_lastAnchoredPosition;

        /// <summary>
        /// The ItemUI of the SightItem that is being dragged and dropped on this ItemUI
        /// We need to store this, and later we use it to add the SightItem to the inventory
        /// </summary>
        private ItemUI m_TempItemUI;
        public ItemUI TempItemUI => m_TempItemUI;

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

            ///<remarks>
            /// Hide the item by default in awake,
            /// as WeaponInventoryUI's ToggleInventoryUI(false) is called in Start
            /// So if Hide() is called in start,
            /// It will call Hide() after Show() in SetItemData
            /// </remarks>
            Hide();     
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                m_Mediator.OnRightClickInputOnAttachmentItemUI(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_lastAnchoredPosition = m_RectTransform.anchoredPosition;
            m_LastParent = m_RectTransform.parent;

            IsDragSuccess = false;

            m_CanvasGroup.blocksRaycasts = false;
            // Set the parent to the canvas so that the UI is not clipped by the parent
            transform.SetParent(m_Mediator.CanvasTransform);
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
                m_RectTransform.SetParent(m_LastParent);
                m_RectTransform.anchoredPosition = m_lastAnchoredPosition;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            if (eventData.pointerDrag.TryGetComponent(out AttachmentItemUI droppedAttachmentItemUI))
            {
                m_Mediator.OnAttachmentItemUIDroppedInAttachmentItemUI(droppedAttachmentItemUI, this);
            }
            else if (eventData.pointerDrag.TryGetComponent(out ItemUI droppedItemUI))
            {
                m_Mediator.OnItemUIDroppedInAttachmentItemUI(droppedItemUI, this);
            }
        }

        public void SetSlotIndex(int index)
        {
            m_SlotIndex = index;
        }

        internal void SetDataAndShowAttachmentItemUI(IWeaponAttachment item)
        {
            m_StoredItem = item;
            m_Icon.sprite = item.ItemData.IconSprite;

            Show();
        }

        internal void ResetDataAndHideAttachmentItemUI()
        {
            m_StoredItem = null;
            m_Icon.sprite = null;

            Hide();
        }

        internal void Show()
        {
            m_CanvasGroup.alpha = 1;
        }

        internal void ShowSlot()
        {
            if (transform.parent.TryGetComponent(out CanvasGroup cg))
            {
                cg.alpha = 1;
                cg.blocksRaycasts = true;
            }
        }

        internal void Hide()
        {
            m_CanvasGroup.alpha = 0;
        }

        internal void HideSlot()
        {
            if (transform.parent.TryGetComponent(out CanvasGroup cg))
            {
                cg.alpha = 0;
                cg.blocksRaycasts = false;
            }
        }
    }
}
