using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "ItemEventChannel", menuName = "ScriptableObjects/Event Channels/ItemEventChannelSO")]
    public class ItemEventChannelSO : EventChannelBaseSO<ItemBase>
    {
    }
}
