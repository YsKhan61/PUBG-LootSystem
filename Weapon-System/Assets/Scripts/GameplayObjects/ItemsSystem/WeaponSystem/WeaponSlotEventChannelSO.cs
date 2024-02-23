using UnityEngine;
using Weapon_System.Utilities;

namespace Weapon_System.GameplayObjects.WeaponSystem
{
    [CreateAssetMenu(fileName = "WeaponSlotEventChannel", menuName = "ScriptableObjects/Event Channels/WeaponSlotEventChannelSO")]
    public class WeaponSlotEventChannelSO : EventChannelBaseSO<WeaponSlotManager.SlotLabel>
    {
    }
}