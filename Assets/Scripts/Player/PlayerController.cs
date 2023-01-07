using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement m_PlayerMovement;

        public void OnMovement(InputAction.CallbackContext _context)
        {
            Debug.Log("On Movemen called");
            m_PlayerMovement.WantedMovement = _context.ReadValue<Vector2>();
            if (_context.phase is InputActionPhase.Canceled or InputActionPhase.Disabled)
            {
                m_PlayerMovement.WantedMovement = Vector2.zero;
            }
        }

        // TODO: Do something else for mouse
        public void OnControllerAimChanged(InputAction.CallbackContext _context)
        {
            
        }

        public void OnShoot(InputAction.CallbackContext _context)
        {
            
        }

        public void OnPlanting(InputAction.CallbackContext _context)
        {
            
        }
    }
}