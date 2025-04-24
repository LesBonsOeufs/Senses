using DG.Tweening;
using UnityEngine;
using Outline = QuickOutline.Outline;

namespace Root
{
    [RequireComponent(typeof(Outline))]
    public class WindowPull_Outlined : WindowPull_AnimatedBase
    {
        private Outline outline;

        void Start()
        {
            outline = GetComponent<Outline>();
        }

        protected override Tweener InteractAnimate(float finalValue)
        {
            outline.DOKill();

            return DOVirtual.Float(outline.OutlineWidth, finalValue, tweenDuration,
                width => outline.OutlineWidth = width).SetTarget(outline);
        }

        protected override Tweener InUseAnimateLoop()
        {
            outline.DOKill();
            outline.OutlineWidth = 10f;
            return DOVirtual.Float(outline.OutlineWidth, 20f, tweenDuration * 2f, width => outline.OutlineWidth = width)
                .SetTarget(outline)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}