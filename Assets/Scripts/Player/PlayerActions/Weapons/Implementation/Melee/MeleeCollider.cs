using System;
using General;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation
{
    [RequireComponent(typeof(Collider))]
    public class MeleeCollider : MonoBehaviour
    {
        private AWeapon m_CurrentWeapon;
        private Collider m_Collider;

        private void Awake()
        {
            enabled = false;
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;
        }

        public void Enable(AWeapon _weapon)
        {
            ChangeState(_weapon, true);
        }

        public void Disable()
        {
            ChangeState(null, false);
        }

        private void ChangeState(AWeapon _weapon, bool _isActivated)
        {
            enabled = _isActivated;
            m_Collider.enabled = _isActivated;
            m_CurrentWeapon = _weapon;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                m_CurrentWeapon?.ApplyDamage(damageable);
            }
        }
    }
}