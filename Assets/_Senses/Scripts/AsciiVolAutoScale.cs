using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using VolFx;

namespace Root
{
    [ExecuteAlways]
    [RequireComponent(typeof(Volume))]
    public class AsciiVolAutoScale : MonoBehaviour
    {
        [InfoBox("Set game scren to \"free aspect\" for seeing result in the scene view."),
         SerializeField] private float baseScale = 1f;
        [SerializeField] private Vector2 referenceScreenSize = new (1920f, 1080f);
        [SerializeField] private float scalingExponant = 1f;

        private AsciiVol asciiVolume;

        [Button("Refresh")]
        private void Start()
        {
            Volume lVolume = GetComponent<Volume>();
            lVolume.profile.TryGet(out asciiVolume);
        }

        private void Update()
        {
            asciiVolume.m_Scale.value = 
                baseScale * Mathf.Pow(referenceScreenSize.x / Screen.width, scalingExponant);
        }
    }
}