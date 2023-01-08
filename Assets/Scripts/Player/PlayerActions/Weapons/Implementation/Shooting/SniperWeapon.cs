using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation.Shooting
{
    public class SniperWeapon : AShootingWeapon
    {
        protected override void ShootRoutine()
        {
            // TODO: Add animation
            ShootPiercing(RayFromShootPosition);
        }
    }
}