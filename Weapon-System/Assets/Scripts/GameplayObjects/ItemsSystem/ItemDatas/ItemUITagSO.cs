using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Tag to identify the UI type of the item
    /// eg: Common, Weapon, HandGun, Throwables, MuzzleAttachment, SightAttachment, MagazineAttachment, StockAttachment, GripAttachment, Backpack etc
    /// An ItemUI of a specific UITag can be stored in the slots which will allow the item to be stored in the specific slots.
    /// </summary>
    /// </remarks>
    [CreateAssetMenu(fileName = "UITag", menuName = "ScriptableObjects/ItemUITagSO")]
    public class ItemUITagSO : ScriptableObject
    {
    }
}