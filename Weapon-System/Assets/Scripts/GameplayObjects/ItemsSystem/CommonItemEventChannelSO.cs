using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "CommonItemEventChannel", menuName = "ScriptableObjects/Event Channels/CommonItemEventChannelSO")]
    public class CommonItemEventChannelSO : EventChannelBaseSO<CommonItem>
    {
    }
}
