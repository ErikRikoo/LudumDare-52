using System;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation
{
    public abstract class AMeleeWeapon : AWeapon
    {
        [SerializeField] private MeleeCollider m_Collider;
        [SerializeField] private float attackDuration = 0.3f;

        public float AttackDuration => attackDuration;
        
        protected override void TriggerAttack()
        {
            m_Collider.Enable(this);
            PlayAnimation();
        }

        protected void OnDamageStop()
        {
            m_Collider.Disable();
        }

        public abstract void PlayAnimation();
    }
}