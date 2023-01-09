using System;
using General;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.PlayerActions.Weapons
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AWeapon : MonoBehaviour
    {
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private float m_Rate;
        [SerializeField] private float m_Damage;
        [SerializeField] private AudioClip m_AttackSound;
        [SerializeField] private AudioClip m_HitSound;
        [SerializeField] private bool m_Piercing;
        
        
        public Sprite Icon => m_Icon;
        public bool Piercing => m_Piercing;
        
        private AudioSource _audioSource;

        private float m_LastAttackTime;
        
        public float RemainingTime => m_Rate - (Time.time - m_LastAttackTime);


        private void Awake()
        {
            m_LastAttackTime = -m_Rate;

            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        public virtual bool HasAmmo => false;

        public void Attack()
        {
            if (Time.time - m_LastAttackTime > m_Rate)
            {
                TriggerAttack();
                _audioSource.pitch = Random.Range(0.6f, 1.1f);                
                _audioSource.PlayOneShot(m_AttackSound);
                m_LastAttackTime = Time.time;
            }
        }

        protected abstract void TriggerAttack();

        public void ApplyDamage(IDamageable _damageable)
        {
            _audioSource.pitch = Random.Range(0.6f, 1.1f);
            _audioSource.PlayOneShot(m_HitSound);
            _damageable.TakeDamage(m_Damage);
        }
    }
}