using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    [CreateAssetMenu(fileName = "WeaponItemEventChannel", menuName = "ScriptableObjects/Event Channels/WeaponItemEventChannelSO")]
    public class WeaponItemEventChannelSO : EventChannelBaseSO<WeaponItem>
    {
    }
}
