
using DG.Tweening;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation
{
    public class PitchFork : AWeapon
    {
        [SerializeField] private AnimationCurve m_Curve;
        [SerializeField] private MeleeCollider m_Collider;
        
        
        protected override void TriggerAttack()
        {
            Debug.Log("Attackings");
            m_Collider.Enable(this);
            Vector3 originalPosition = transform.localPosition;

            float duration = 0.3f;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                transform.DOLocalMove(transform.localPosition + new Vector3(0, 0, 1), duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => m_Collider.Disable())
                );
            sequence.Append(
                    transform.DOLocalMove(originalPosition, duration)
                        .SetEase(Ease.Linear)
            );
            sequence.Play();
            // Play animation
        }
    }
}