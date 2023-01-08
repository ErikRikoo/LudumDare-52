using UnityEngine;

namespace General
{
    public class LogDamage : MonoBehaviour, IDamageable
    {
        public void TakeDamage(float amount)
        {
            Debug.Log($"I take damage: {amount}");
        }

        public event InformAttackersAboutDeath InformAboutDeath;
    }
}