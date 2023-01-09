using System.Collections.Generic;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation.Shooting
{
    public class ShotgunWeapon : AShootingWeapon
    {
        [Range(0, 180)]
        [SerializeField] private float m_HalfShootingAngle;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Vector3 forward = m_ShootPosition.forward;
            Vector3 left = Quaternion.Euler(0, -m_HalfShootingAngle, 0) * forward;
            Vector3 right = Quaternion.Euler(0, m_HalfShootingAngle, 0) * forward;
            Gizmos.DrawLine(m_ShootPosition.position, m_ShootPosition.position + left);
            Gizmos.DrawLine(m_ShootPosition.position, m_ShootPosition.position + right);
            Gizmos.color = Color.red;
            foreach (var ray in ShootRay)
            {
                Gizmos.DrawRay(ray);
            }

            Gizmos.color = Color.green;
        }
        #endif

        protected override void ShootRoutine()
        {
            foreach (var ray in ShootRay)
            {
                Shoot(ray);
            }
        }

        private IEnumerable<Ray> ShootRay
        {
            get
            {
                Vector3 startShootingForward = Quaternion.Euler(0, -m_HalfShootingAngle, 0) * m_ShootPosition.forward;
                float angle = (m_HalfShootingAngle * 2) * (1f / (m_AmmoPerShoot - 1));
                for (int i = 0; i < m_AmmoPerShoot; ++i)
                {
                    yield return new Ray(
                        m_ShootPosition.position,
                        Quaternion.Euler(0, i * angle, 0) * startShootingForward
                    );
                }
            }
        }
    }
}