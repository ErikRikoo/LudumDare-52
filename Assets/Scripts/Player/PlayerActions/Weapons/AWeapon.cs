using System;
using General;
using UnityEngine;

namespace Player.PlayerActions.Weapons
{
    public abstract class AWeapon : MonoBehaviour
    {
        [SerializeField] private float m_Rate;
        [SerializeField] private float m_Damage;
        

        private float m_LastAttackTime;

        private void Awake()
        {
            m_LastAttackTime = -m_Rate;
        }

        public void Attack()
        {
            if (Time.time - m_LastAttackTime > m_Rate)
            {
                TriggerAttack();
                m_LastAttackTime = Time.time;
            }
        }

        protected abstract void TriggerAttack();

        public void ApplyDamage(IDamageable _damageable)
        {
            _damageable.TakeDamage(m_Damage);
        }
    }
}