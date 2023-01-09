using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.PlayerActions.Weapons.Implementation.Shooting
{
    public class RifleWeapon : AShootingWeapon
    {
        [Range(0, 180)]
        [SerializeField] private float m_HalfShootingAngle;
        
        protected override void ShootRoutine()
        {
            Vector3 startShootingForward = Quaternion.Euler(0, -m_HalfShootingAngle, 0) * m_ShootPosition.forward;
            float angle = Random.Range(-m_HalfShootingAngle, m_HalfShootingAngle);
            Ray ray = new Ray(
                m_ShootPosition.position,
                Quaternion.Euler(0, angle, 0) * startShootingForward
            );
            Shoot(ray);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 forward = m_ShootPosition.forward;
            Vector3 left = Quaternion.Euler(0, -m_HalfShootingAngle, 0) * forward;
            Vector3 right = Quaternion.Euler(0, m_HalfShootingAngle, 0) * forward;
            Gizmos.DrawLine(m_ShootPosition.position, m_ShootPosition.position + left);
            Gizmos.DrawLine(m_ShootPosition.position, m_ShootPosition.position + right);
        }
        #endif
    }
    
}