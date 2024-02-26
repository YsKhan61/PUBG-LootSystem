using UnityEngine;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// This is the data of the gun item.
    /// Different guns such as M416, AKM, etc. will have different data.
    /// </summary>
    [CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/ItemDatas/GunDataSO", order = 0)]
    public class GunDataSO : ItemDataSO
    {
        /*[SerializeField, Tooltip("How much will the player camera zoom in this iron sight ADS")]
        float m_ADSZoomValue;
        public float ADSZoomValue => m_ADSZoomValue;*/
    }
}