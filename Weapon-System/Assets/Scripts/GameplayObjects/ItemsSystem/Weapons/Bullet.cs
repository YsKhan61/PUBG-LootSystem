using System;
using System.Collections;
using UnityEngine;
using Weapon_System.Utilities;


namespace Weapon_System.GameplayObjects.ItemsSystem
{
    public class Bullet : MonoBehaviour, ISpawnable
    {
        [SerializeField]
        float initialSpeed;

        [SerializeField]
        Rigidbody m_Rigidbody;

        public string Name => "Bullet";         // Later these will be given by Scriptable Object data

        public GameObject GameObject => gameObject;

        [SerializeField]
        int m_PoolSize;
        public int PoolSize => m_PoolSize;

        private void OnDisable()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            m_Rigidbody.position = position;
            transform.rotation = rotation;
            m_Rigidbody.rotation = rotation;
        }

        public void Fire()
        {
            m_Rigidbody.AddForce(transform.forward * initialSpeed, ForceMode.Impulse);

            StartCoroutine(DeactivateAfterTime());
        }

        IEnumerator DeactivateAfterTime()
        {
            yield return new WaitForSeconds(1);
            PoolManager.Instance.ReleaseObjectToPool(this);
        }
    }
}

