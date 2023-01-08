
using DG.Tweening;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation
{
    public class PitchFork : AMeleeWeapon
    {
        [SerializeField] private Ease easingAttack = Ease.Linear;  
        [SerializeField] private Ease easingReturnToDefault = Ease.Linear;

        public override void PlayAnimation()
        {
            Vector3 originalPosition = transform.localPosition;

            float duration = AttackDuration;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                transform.DOLocalMove(transform.localPosition + new Vector3(0, 0, 1), duration)
                    .SetEase(easingAttack)
                    .OnComplete(OnDamageStop)
                );
            sequence.Append(
                    transform.DOLocalMove(originalPosition, duration)
                        .SetEase(easingReturnToDefault)
            );
            sequence.Play();
        }
    }
}