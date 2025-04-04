using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(CanvasGroup))]
    public class WindowOpenCloseAnim : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private float scaleDuration = 0.25f;
        [SerializeField] private float startScale = 0.75f;
        [SerializeField] private bool destroyOnOut = false;

        private Tween anim;

        private void Start()
        {
            In();
        }

        [Button]
        public void In()
        {
            anim?.Kill();

            CanvasGroup lCanvasGroup = GetComponent<CanvasGroup>();
            lCanvasGroup.blocksRaycasts = false;
            lCanvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * startScale;

            anim = DOTween.Sequence(this)
                .Append(lCanvasGroup.DOFade(1f, fadeDuration))
                .Join(transform.DOScale(Vector3.one, scaleDuration))
                .OnComplete(() =>
                {
                    lCanvasGroup.blocksRaycasts = true;
                });
        }

        [Button]
        public void Out()
        {
            anim?.Kill();

            CanvasGroup lCanvasGroup = GetComponent<CanvasGroup>();

            anim = DOTween.Sequence(this)
                .Append(lCanvasGroup.DOFade(0f, fadeDuration))
                .Join(transform.DOScale(Vector3.one * startScale, scaleDuration))
                .OnComplete(() =>
                {
                    if (destroyOnOut)
                        Destroy(gameObject);
                });
        }
    }
}