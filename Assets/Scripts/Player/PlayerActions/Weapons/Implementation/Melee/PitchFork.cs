
using DG.Tweening;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation
{
    public class PitchFork : AWeapon
    {
        [SerializeField] private AnimationCurve m_Curve;
        [SerializeField] private MeleeCollider m_Collider;
        [SerializeField] private float attackDuration = 0.3f;
        [SerializeField] private Ease easingAttack = Ease.Linear;  
        [SerializeField] private Ease easingReturnToDefault = Ease.Linear;  
        
        
        protected override void TriggerAttack()
        {
            Debug.Log("Attackings");
            m_Collider.Enable(this);
            Vector3 originalPosition = transform.localPosition;

            float duration = attackDuration;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                transform.DOLocalMove(transform.localPosition + new Vector3(0, 0, 1), duration)
                    .SetEase(easingAttack)
                    .OnComplete(() => m_Collider.Disable())
                );
            sequence.Append(
                    transform.DOLocalMove(originalPosition, duration)
                        .SetEase(easingReturnToDefault)
            );
            sequence.Play();
            // Play animation
        }
    }
}