using System;
using General;
using UnityEditor;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation.Shooting
{
    public abstract class AShootingWeapon : AWeapon
    {
        [SerializeField] private int m_Ammo;
        [SerializeField] protected int m_AmmoPerShoot;
        [SerializeField] private ParticleSystem[] m_ShootingEffects;
        [SerializeField] protected Transform m_ShootPosition;
        [SerializeField] private LayerMask m_Layer;
        [SerializeField] private Bullet m_BulletPrefab;
        
        

        protected Ray RayFromShootPosition => new Ray(m_ShootPosition.position, m_ShootPosition.forward);
        
        public int MaxAmmo => m_Ammo;
        
        [HideInInspector]
        public int m_CurrentAmmo;

        public override bool HasAmmo => true;
        protected override void TriggerAttack()
        {
            m_CurrentAmmo -= m_AmmoPerShoot;
            GameEvents.OnAmmoChanged?.Invoke(m_CurrentAmmo);
            if (m_CurrentAmmo <= 0)
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
            var bullet = Instantiate(m_BulletPrefab, m_ShootPosition.position, m_ShootPosition.rotation);
            bullet.Damage = (int) m_Damage;
            bullet.ShouldDestroyOnTrigger = true;
            return;
            
            int count = Physics.RaycastNonAlloc(_ray, m_RaycastBuffer, 1000, m_Layer);

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
            var bullet = Instantiate(m_BulletPrefab, m_ShootPosition.position, m_ShootPosition.rotation);
            bullet.Damage = (int) m_Damage;
            bullet.ShouldDestroyOnTrigger = false;
            return;
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