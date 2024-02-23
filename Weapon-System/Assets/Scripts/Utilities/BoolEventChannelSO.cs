using UnityEngine;

namespace Weapon_System.Utilities
{
    /// <summary>
    /// This can be used as a VoidEventChannel as well. Just ignore the value.
    /// </summary>
    [CreateAssetMenu(fileName = "BoolEventChannel", menuName = "ScriptableObjects/Event Channels/BoolEventChannelSO")]
    public class BoolEventChannelSO : EventChannelBaseSO<bool>
    {
    }
}