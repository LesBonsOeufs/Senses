using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class WindowPull_TMP : WindowPull_AnimatedBase
    {
        private TextMeshProUGUI tmp;
        private Material tmpMaterial;

        private float Glow
        {
            get => tmpMaterial.GetFloat(ShaderUtilities.ID_GlowOuter);

            set
            {
                tmpMaterial.SetFloat(ShaderUtilities.ID_GlowInner, value);
                tmpMaterial.SetFloat(ShaderUtilities.ID_GlowOuter, value);
            }
        }

        private void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            tmpMaterial = tmp.fontMaterial;
        }

        protected override Tweener InteractAnimate(float finalValue)
        {
            tmp.DOKill();

            return DOVirtual.Float(Glow, finalValue, tweenDuration,
                glow => Glow = glow).SetTarget(tmp);
        }

        protected override Tweener InUseAnimateLoop()
        {
            tmp.DOKill();
            Glow = 0f;

            return DOVirtual.Float(Glow, loopValue, tweenDuration * 3f, glow => Glow = glow)
                .SetTarget(tmp)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}