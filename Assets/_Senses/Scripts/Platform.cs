using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private float tweenDuration = 3f;
        [SerializeField] private Vector3 addedPosition = Vector3.up;

        private bool isOn = false;

        [Button]
        public void Activate()
        {
            if (isOn)
                return;

            isOn = true;

            transform.DOMove(transform.position + addedPosition, tweenDuration)
                .SetEase(Ease.OutSine);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + addedPosition);
            Gizmos.DrawSphere(transform.position + addedPosition, 0.1f);
        }
    }
}