using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation.Shooting
{
    public class PistolWeapon : AShootingWeapon
    {
        protected override void ShootRoutine()
        {
            // TODO: Add animation
            Shoot(RayFromShootPosition);
        }
    }
}