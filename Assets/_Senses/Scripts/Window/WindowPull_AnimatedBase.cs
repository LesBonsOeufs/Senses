using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    public abstract class WindowPull_AnimatedBase : WindowPullBase
    {
        [Foldout("Animation"), SerializeField] protected float tweenDuration = 0.2f;
        [Foldout("Animation"), SerializeField] private float hoverValue = 5f;
        [Foldout("Animation"), SerializeField] private float interactValue = 10f;

        protected abstract Tweener InteractAnimate(float finalValue);
        protected abstract Tweener InUseAnimateLoop();

        protected override void Window_OnStartOut(float duration)
        {
            InteractAnimate(0f);
            base.Window_OnStartOut(duration);
        }

        protected override void MakeWindow(Transform parent, Vector2 position)
        {
            base.MakeWindow(parent, position);
            InUseAnimateLoop();
        }

        public override void In()
        {
            InteractAnimate(hoverValue);
        }

        public override void Out()
        {
            InteractAnimate(0f);
        }

        public override void InteractOn()
        {
            InteractAnimate(interactValue);
        }

        public override void InteractOff()
        {
            InteractAnimate(hoverValue);
        }
    }
}