using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class SightItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField]
        ItemUIType m_ItemUIType;
        public ItemUIType ItemUIType => m_ItemUIType;

        [SerializeField]
        Image m_Icon;

        [SerializeField]
        SightInventoryUIMediator m_SightInventoryUI;

        /*[SerializeField, FormerlySerializedAs("m_InventoryUI")]
        private WeaponInventoryUI m_WeaponInventoryUI;*/

        public int SlotIndex { get; private set; }

        private SightAttachmentItem m_StoredSightItem;
        public SightAttachmentItem StoredSightItem => m_StoredSightItem;

        [HideInInspector]
        public bool IsDragSuccess;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;
        private Vector2 m_lastAnchoredPosition;

        /// <summary>
        /// The ItemUI of the SightItem that is being dragged and dropped on this SightItemUI
        /// We need to store this, and later we use it to add the SightItem to the inventory
        /// </summary>
        private ItemUI m_ItemUI;

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
                // Remove the SightItem from the gun
                if (!m_SightInventoryUI.WeaponInventoryUI.TryGetGunItemFromWeaponInventoryUI(
                    SlotIndex, out GunItem gun))
                {
                    Debug.LogError("This should not happen, as a SightItemUI can only be present in the inventory if it is attached to a GunItem");
                    return;
                }

                gun.DetachSight();

                // Store back the SightItem in the inventory
                m_SightInventoryUI.AddSightItemToInventory(m_StoredSightItem);

                // Make sure to reset the ItemUI's position to the last anchored position
                ResetAndShowItemUI();

                ResetItemDataAndHide();
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
            if (eventData.pointerDrag == null)
                return;

            if (eventData.pointerDrag.TryGetComponent(out SightItemUI sightItemUI))
            {
                // another SightItemUI is being dropped on this SightItemUI
                if (sightItemUI.ItemUIType != ItemUIType)
                {
                    return;
                }
                m_SightInventoryUI.SwapSightItemUIs(sightItemUI.SlotIndex, SlotIndex);
            }
            else if (eventData.pointerDrag.TryGetComponent(out ItemUI itemUI))
            {
                // an ItemUI is being dropped on this SightItemUI

                // NOTE - We need to do two checks here -
                // 1. Check if the ItemUI is of the same type as this SightItemUI -> SightItemUI can be dropped on another SightItemUI or SightSlotUI
                // 2. Check if the SightItem can be attached to the GunItem -> 8x scope (SightItem) cannot be attached to a Pistol (GunItem)

                // Check if the ItemUI is of the same type as this SightItemUI
                if (itemUI.ItemData.UIType != ItemUIType)
                {
                    return;
                }

                // if yes,
                // Try to get GunItemData from the WeaponInventoryUI
                if (!m_SightInventoryUI.WeaponInventoryUI.TryGetGunItemFromWeaponInventoryUI(
                    SlotIndex, out GunItem gun))
                {
                    return;
                }

                // if GunItem is found, check if the SightItem of dropped SightItemUI can be attached to the GunItem
                if (!gun.IsSightTypeCompatible(itemUI.ItemData.Type))
                {
                    return;
                }

                // if yes,
                // then check if the GunItem already has a SightItem attached to it (also SightItem can be present in this class)
                if (gun.SightAttachment != null || StoredSightItem != null)
                {
                    if (gun.SightAttachment != StoredSightItem as ISightAttachment)
                    {
                        Debug.LogError("This should not happen as the Gun's sight attachment must match with the stored Sight Attachment Item");
                        return;
                    }

                    // detach it from GunItem
                    gun.DetachSight();

                    // remove the SightItemUI from SightSlotUI through SightInventoryUI
                    m_SightInventoryUI.RemoveSightItemUIFromSightSlotUI(m_StoredSightItem);

                    // add the SightItem to the inventory
                    m_SightInventoryUI.AddSightItemToInventory(m_StoredSightItem);
                    
                    if (m_ItemUI == null)
                    {
                        // If the ItemUI is null, then throw an error as every SightItemUI should have an ItemUI
                        Debug.LogError("This should not be null");
                    }
                    else
                    {
                        // Show the ItemUI if it is not null, and block raycast, and fallback to last position
                        ResetAndShowItemUI();
                    }
                }

                // Then remove the SightItem of dropped ItemUI from the inventory using an event
                m_SightInventoryUI.RemoveSightItemFromInventory(itemUI.Item);

                // Then hide the ItemUI and store it in a member variable
                HideItemUI(itemUI);
                
                // then set the ItemUI's datas to this SightItemUI and Show it
                SetItemDataAndShow(itemUI.Item as SightAttachmentItem);

                // then set this attachment to the GunItem
                gun.AttachSight(itemUI.Item as SightAttachmentItem);
            }
        }

        public void SetSlotIndex(int index)
        {
            SlotIndex = index;
        }

        public void SetItemDataAndShow(SightAttachmentItem item)
        {
            m_StoredSightItem = item;
            m_Icon.sprite = item.ItemData.IconSprite;

            Show();
        }

        void ResetItemDataAndHide()
        {
            m_StoredSightItem = null;
            m_Icon.sprite = null;

            Hide();
        }

        void HideItemUI(ItemUI itemUI)
        {
            itemUI.IsDragSuccess = true;            // Set the drag success to true, so that the OnEndDrag of ItemUI doesn't make it visible again or fallback to last position
            itemUI.Hide();
            itemUI.UnblockRaycast();
            m_ItemUI = itemUI;
        }

        void ResetAndShowItemUI()
        {
            if (m_ItemUI == null)
            {
                // If the ItemUI is null, then throw an error as every SightItemUI should have an ItemUI
                Debug.LogError("This should not be null");
            }
            else
            {
                // Show the ItemUI if it is not null, and block raycast, and fallback to last position
                m_ItemUI.Show();
                m_ItemUI.BlockRaycast();
                m_ItemUI.FallbackToLastPosition();
            }
        }

        private void Show()
        {
            m_CanvasGroup.alpha = 1;
        }

        private void Hide()
        {
            m_CanvasGroup.alpha = 0;
        }
    }
}
