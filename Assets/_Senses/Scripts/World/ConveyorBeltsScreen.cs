using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Root
{
    public class ConveyorBeltsScreen : MonoBehaviour
    {
        [SerializeField] private GameObject correct;
        [SerializeField] private GameObject incorrect;
        [SerializeField] private float showDuration = 3f;

        private void Start()
        {
            correct.SetActive(false);
            incorrect.SetActive(false);
        }

        [Button]
        public void ShowCorrect()
        {
            incorrect.SetActive(false);
            DOTween.Kill(incorrect);

            correct.SetActive(true);
            DOVirtual.DelayedCall(showDuration, () => correct.SetActive(false)).SetTarget(correct);
            //Add SFX
        }

        [Button]
        public void ShowIncorrect()
        {
            correct.SetActive(false);
            DOTween.Kill(correct);

            incorrect.SetActive(true);
            DOVirtual.DelayedCall(showDuration, () => incorrect.SetActive(false)).SetTarget(incorrect);
            //Add SFX
        }
    }
}