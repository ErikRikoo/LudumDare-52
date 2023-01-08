using Player.PlayerActions;
using Player.PlayerActions.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_PlayerStats;
        
        public void OnMovement(InputAction.CallbackContext _context)
        {
            m_PlayerStats.PlayerMovement.WantedMovement = _context.ReadValue<Vector2>();
            if (_context.phase is InputActionPhase.Canceled or InputActionPhase.Disabled)
            {
                m_PlayerStats.PlayerMovement.WantedMovement = Vector2.zero;
            }
        }

        public void OnShoot(InputAction.CallbackContext _context)
        {
            m_PlayerStats.CurrentWeapon.Attack();
        }

        public void OnPlanting(InputAction.CallbackContext _context)
        {
            m_PlayerStats.PlantingBehavior.PlantSeed();
        }
    }
}