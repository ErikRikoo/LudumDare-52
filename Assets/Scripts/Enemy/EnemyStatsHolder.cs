using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyStatsHolder", menuName = "Enemies/EnemyStatsHolder")]
    public class EnemyStatsHolder : ScriptableObject
    {
    
    
        //Base stats for the weapon
        [SerializeField] float damage;
        public float Damage { get => damage; private set => damage = value; }
    
        [SerializeField] float range;
        public float Range { get => range; private set => range = value; }

        
            
        [SerializeField] float visionRange;
        public float VisionRange { get => visionRange; private set => visionRange = value; }

        [SerializeField] float speed;
        public float Speed { get => speed; private set => speed = value; }

        [SerializeField] float attackSpeed;
        public float AttackSpeed { get => attackSpeed; private set => attackSpeed = value; }

        [SerializeField] private float maxHealth;
        public float MaxHealth { get => maxHealth; private set => maxHealth = value;
        }



    }
}