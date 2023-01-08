using DG.Tweening;
using UnityEngine;

namespace Player.PlayerActions.Weapons.Implementation
{
    public class SwordWeapon : AMeleeWeapon
    {
        [Range(0, 360)]
        [SerializeField] private float m_AttackAngle;
        
        [SerializeField] private Ease easingAttack = Ease.Linear;  
        [SerializeField] private Ease easingReturnToDefault = Ease.Linear;
        
        public override void PlayAnimation()
        {
            Quaternion originalRotation = transform.localRotation;

            float duration = AttackDuration;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                transform.DOLocalRotate(
                    (originalRotation * Quaternion.Euler(0, m_AttackAngle, 0)).eulerAngles, duration)
                    .SetEase(easingAttack)
                    .OnComplete(OnDamageStop)
            );
            sequence.Append(
                transform.DOLocalRotate(originalRotation.eulerAngles, duration)
                    .SetEase(easingReturnToDefault)
            );
            sequence.Play();
        }
    }
}