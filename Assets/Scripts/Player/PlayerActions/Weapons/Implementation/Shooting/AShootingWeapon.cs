using System;
using UnityEditor;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation.Shooting
{
    public abstract class AShootingWeapon : AWeapon
    {
        [SerializeField] private int m_Ammo;
        [SerializeField] private int m_AmmoPerShoot;
        [SerializeField] private ParticleSystem[] m_ShootingEffects;
        

        public int MaxAmmo => m_Ammo;

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

        protected abstract void ShootRoutine();

        private void OnEnable()
        {
            m_CurrentAmmo = m_Ammo;
        }
    }
}