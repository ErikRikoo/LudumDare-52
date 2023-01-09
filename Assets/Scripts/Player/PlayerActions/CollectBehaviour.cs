using System;
using Collectable;
using DG.Tweening;
using UnityEngine;

namespace Player.PlayerActions
{
    [RequireComponent(typeof(SphereCollider))]
    public class CollectBehaviour : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;

        [Min(0)]
        [SerializeField] private float m_DetectionRadius;

        [Min(0)]
        [SerializeField] private float m_TweenAnimationDuration = 0.1f;

        [SerializeField] private Ease m_EaseType = Ease.InQuad;

        private void Start()
        {
            var collider = GetComponent<SphereCollider>();
            collider.radius = m_DetectionRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ICollectable>(out var collectable))
            {
                collectable.OnCollected(m_Stats.gameObject);

                other.transform.DOMove(transform.position, m_TweenAnimationDuration)
                    .SetEase(m_EaseType)
                    .OnComplete(() =>
                    {
                        collectable.OnCollected(m_Stats.gameObject);
                        Destroy(other.gameObject);
                    });
            }
        }
    }
}