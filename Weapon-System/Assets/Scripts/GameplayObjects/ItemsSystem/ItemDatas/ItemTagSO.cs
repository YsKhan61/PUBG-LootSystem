using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Tag for each item. The name of the scriptable object will be the name of the item.
    /// </summary>
    /// <remarks>
    /// We create tags, for many reasons -
    ///     1. To check if the item is allowed to be attached to the gun or not,
    ///         we just need to check if the tag of the sight item is in the allowed tags of the gun.
    ///         In this way, we are not exposing any other data of the sight to the gun item while doing the check.
    /// </remarks>
    [CreateAssetMenu(fileName = "Tag", menuName = "ScriptableObjects/ItemTag", order = 0)]
    public class ItemTagSO : ScriptableObject
    {
    }
}