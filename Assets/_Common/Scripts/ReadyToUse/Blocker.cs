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

        [Button]
        public void Open()
        {
            left.DOBlendableRotateBy(Vector3.up * -90f, tweenDuration);
            right.DOBlendableRotateBy(Vector3.up * 90f, tweenDuration);
        }

        [Button]
        public void Close()
        {
            left.DOBlendableRotateBy(Vector3.up * 90f, tweenDuration);
            right.DOBlendableRotateBy(Vector3.up * -90f, tweenDuration);
        }
    }
}