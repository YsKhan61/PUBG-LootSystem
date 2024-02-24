using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "ItemDataEventChannel", menuName = "ScriptableObjects/Event Channels/ItemDataEventChannelSO")]
    public class ItemDataEventChannelSO : EventChannelBaseSO<ItemDataSO>
    {
    }
}
