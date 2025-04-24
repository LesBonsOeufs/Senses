using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Outline = QuickOutline.Outline;

namespace Root
{
    [RequireComponent(typeof(Outline))]
    public class WindowPull_Outlined : WindowPullBase
    {
        [Foldout("Outline"), SerializeField] private float tweenDuration = 0.2f;
        [Foldout("Outline"), SerializeField] private float hoverWidth = 5f;
        [Foldout("Outline"), SerializeField] private float interactWidth = 10f;

        private Outline outline;

        void Start()
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }

        private Tweener TweenOutline(float finalWidth)
        {
            outline.DOKill();

            if (finalWidth != 0f)
                outline.enabled = true;

            return DOVirtual.Float(outline.OutlineWidth, finalWidth, tweenDuration,
                width => outline.OutlineWidth = width).SetTarget(outline)
                .OnComplete(() =>
                {
                    if (finalWidth == 0f)
                        outline.enabled = false;
                });
        }

        protected override void Window_OnStartOut(float duration)
        {
            TweenOutline(0f);
            base.Window_OnStartOut(duration);
        }

        public override void In()
        {
            TweenOutline(hoverWidth);
        }

        public override void Out()
        {
            TweenOutline(0f);
        }

        public override void InteractOn()
        {
            TweenOutline(interactWidth);
        }

        public override void InteractOff()
        {
            TweenOutline(hoverWidth);
        }

        protected override void MakeWindow(Transform parent, Vector2 position)
        {
            base.MakeWindow(parent, position);

            outline.DOKill();
            outline.OutlineWidth = hoverWidth;
            DOVirtual.Float(outline.OutlineWidth, interactWidth, tweenDuration * 2f, width => outline.OutlineWidth = width)
                .SetTarget(outline)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}