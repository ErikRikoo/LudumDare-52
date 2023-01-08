using Player.PlayerActions;
using UnityEngine;
using Utilities;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;

        [Header("Movement Settings")]
        [SerializeField] private float m_Speed;
        [Min(0)]
        [SerializeField] private float m_Damping = 1;

        public Vector2 WantedMovement
        {
            get => m_WantedMovement;
            set => m_WantedMovement = value;
        }

        private Vector2 m_WantedMovement;
        
        private Vector2 m_CurrentMovement;

        private Rigidbody m_Rigidbody;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // TODO: Use slerp?
            m_CurrentMovement = Vector2.LerpUnclamped(m_CurrentMovement, m_WantedMovement, m_Damping * Time.deltaTime);
            m_Rigidbody.velocity = Velocity;
            // TODO: Use int key (AnimatorParam from Naughty Attribute)
            if (m_Stats.Animator != null)
            {
                m_Stats.Animator.SetFloat("Speed", m_CurrentMovement.magnitude);
            }
        }

        private Vector3 Velocity => m_CurrentMovement.X0Y() * m_Speed;
    }
}