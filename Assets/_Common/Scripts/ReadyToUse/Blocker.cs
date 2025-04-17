using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    public class Blocker : MonoBehaviour
    {
        [SerializeField] private Transform left;
        [SerializeField] private Transform right;
        [SerializeField] private float tweenDuration = 0.5f;

        private bool isOpen = false;

        [Button]
        public void Open()
        {
            if (isOpen)
                return;

            left.DOBlendableRotateBy(Vector3.up * -90f, tweenDuration);
            right.DOBlendableRotateBy(Vector3.up * 90f, tweenDuration);
            isOpen = true;
        }

        [Button]
        public void Close()
        {
            if (!isOpen)
                return;

            left.DOBlendableRotateBy(Vector3.up * 90f, tweenDuration);
            right.DOBlendableRotateBy(Vector3.up * -90f, tweenDuration);
            isOpen = false;
        }
    }
}