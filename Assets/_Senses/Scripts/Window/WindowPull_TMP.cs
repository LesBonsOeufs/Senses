using TMPro;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class WindowPull_TMP : WindowPullBase
    {
        private TextMeshProUGUI tmp;

        private void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
        }

        public override void In()
        {
            tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.1f);
        }

        public override void Out()
        {
            tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
        }

        public override void InteractOff()
        {
            throw new System.NotImplementedException();
        }

        public override void InteractOn()
        {
            throw new System.NotImplementedException();
        }
    }
}