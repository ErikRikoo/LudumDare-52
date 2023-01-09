using System;
using System.Collections;
using General;
using UnityEngine;
using UnityEngine.Android;

namespace Player.PlayerActions.Weapons.Implementation
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float m_Speed;
        
        
        public bool ShouldDestroyOnTrigger;
        public int Damage;

        private void FixedUpdate()
        {
            transform.position += (m_Speed * Time.fixedDeltaTime) * transform.forward;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Triggered biiiitch");
            if (other.TryGetComponent<IDamageable>(out var _damageable))
            {
                _damageable.TakeDamage(Damage);
                if (ShouldDestroyOnTrigger)
                {
                    // TODO: Use a pool
                    Destroy(gameObject);
                }
            }
        }
    }
}