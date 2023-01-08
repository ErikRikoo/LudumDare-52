using Player.PlayerActions.Harvest;
using Player.PlayerActions.Weapons;
using UnityEngine;

namespace Player.PlayerActions
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private PlayerMovement m_PlayerMovement;

        public PlayerMovement PlayerMovement => m_PlayerMovement;


        [SerializeField] private PlantingBehavior m_PlantingBehavior;

        public PlantingBehavior PlantingBehavior => m_PlantingBehavior;

        [SerializeField] private AWeapon m_CurrentWeapon;

        public AWeapon CurrentWeapon
        {
            get => m_CurrentWeapon;
            set => m_CurrentWeapon = value;
        }

        [SerializeField] private PlayerInventory m_Inventory;

        public PlayerInventory Inventory => m_Inventory;


        [SerializeField] private Animator m_Animator;
        public Animator Animator => m_Animator;

        [SerializeField] private HarvestBehaviour m_HarvestBehavior;
        public HarvestBehaviour HarvestBehaviour => m_HarvestBehavior;


    }
}