using UnityEngine;


namespace Weapon_System.Utilities
{
    [CreateAssetMenu(fileName = "ItemTagEventChannel", menuName = "ScriptableObjects/Event Channels/ItemTagEventChannel")]
    public class ItemTagEventChannelSO : EventChannelBaseSO<ItemTagSO>
    {
        ItemTagSO m_ItemTag;

        public override void RaiseEvent(ItemTagSO tag)
        {
            m_ItemTag = tag;
            OnEventRaised?.Invoke(m_ItemTag);
        }
    }
}
