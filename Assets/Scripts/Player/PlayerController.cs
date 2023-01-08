using System.Collections;
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

        private Coroutine m_ShootingCoroutine;
        public void OnShoot(InputAction.CallbackContext _context)
        {
            if (_context.phase is InputActionPhase.Started or InputActionPhase.Performed)
            {
                if (m_ShootingCoroutine == null)
                {
                    m_ShootingCoroutine = StartCoroutine(c_Shoot());
                }
            }
            else
            {
                if (m_ShootingCoroutine != null)
                {
                    StopCoroutine(m_ShootingCoroutine);
                }
                m_ShootingCoroutine = null;
            }
        }

        private IEnumerator c_Shoot()
        {
            while (true)
            {
                float remainingTime = m_PlayerStats.CurrentWeapon.RemainingTime;
                if (remainingTime > 0)
                {
                    yield return new WaitForSeconds(remainingTime);
                }
                m_PlayerStats.CurrentWeapon.Attack();
            }
        }

        public void OnPlanting(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started)
            {
                m_PlayerStats.PlantingBehavior.PlantSeed();
            }
        }
        
        public void OnSeedScroll(InputAction.CallbackContext _context)
        {
            
        }

        public void OnSeedChange(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started)
            {
                m_PlayerStats.Inventory.CurrentSeed = int.Parse(_context.control.name) - 1;
            }
        }
        
        public void OnHarvest(InputAction.CallbackContext _context)
        {
            if (_context.phase == InputActionPhase.Started)
            {
                m_PlayerStats.HarvestBehaviour.TryHarvestPlants();
            }
        }
    }
}