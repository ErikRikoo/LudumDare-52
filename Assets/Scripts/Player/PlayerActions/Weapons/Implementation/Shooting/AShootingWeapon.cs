using System;
using General;
using UnityEditor;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation.Shooting
{
    public abstract class AShootingWeapon : AWeapon
    {
        [SerializeField] private int m_Ammo;
        [SerializeField] private int m_AmmoPerShoot;
        [SerializeField] private ParticleSystem[] m_ShootingEffects;
        [SerializeField] private Transform m_ShootPosition;

        protected Ray RayFromShootPosition => new Ray(m_ShootPosition.position, m_ShootPosition.forward);
        
        public int MaxAmmo => m_Ammo;
        
        [HideInInspector]
        public int m_CurrentAmmo;

        public override bool HasAmmo => true;
        protected override void TriggerAttack()
        {
            m_Ammo -= m_AmmoPerShoot;
            GameEvents.OnAmmoChanged?.Invoke(m_Ammo);
            if (m_Ammo <= 0)
            {
                GameEvents.OnWeaponAmmoEntirelyConsumed?.Invoke();
            }

            ShootRoutine();
            foreach (var effect in m_ShootingEffects)
            {
                effect.Play();
            }
        }

        private RaycastHit[] m_RaycastBuffer = new RaycastHit[16];
        
        public void Shoot(Ray _ray)
        {
            int count = Physics.RaycastNonAlloc(_ray, m_RaycastBuffer);

            for (int i = 0; i < count; ++i)
            {
                if (m_RaycastBuffer[i].collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(m_AmmoPerShoot);
                    return;
                }
            }
        }

        public void ShootPiercing(Ray _ray)
        {
            int count = Physics.RaycastNonAlloc(_ray, m_RaycastBuffer);

            for (int i = 0; i < count; ++i)
            {
                if (m_RaycastBuffer[i].collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(m_AmmoPerShoot);
                }
            }
        }
        
        protected abstract void ShootRoutine();

        private void OnEnable()
        {
            m_CurrentAmmo = m_Ammo;
        }
    }
}