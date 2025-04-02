using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TMPro
{
    public class TMP_Animated : TextMeshProUGUI
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float timeBtwWord = 2f;
        [SerializeField] private AudioSource writingSFX = null;

        public UnityEvent onDialogueEnd;
        private IEnumerator readingRoutine;

        public override string text
        {
            get => base.text;

            set
            {
                ReadText(value);
            }
        }

        /// <summary>
        /// In seconds
        /// </summary>
        public float TimeForCompletingCurrentText { get; private set; }

        protected override void OnEnable()
        {
            //Read current text
            text = text;
            base.OnEnable();
        }

        private void ReadText(string displayText)
        {
            if (readingRoutine is not null) StopCoroutine(readingRoutine);

            if (writingSFX != null)
                writingSFX.Play();

            maxVisibleCharacters = 0;

            base.text = displayText;
            readingRoutine = ReadingText();
            StartCoroutine(readingRoutine);

            IEnumerator ReadingText()
            {
                if (base.text == string.Empty) yield break;

                TimeForCompletingCurrentText = 0f;
                string[] lWordTexts = base.text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                float lScaledTimeBtwCharacters = 1f / speed;
                float lScaledTimeBtwWords = timeBtwWord / speed;

                for (int i = 0; i < lWordTexts.Length; i++)
                {
                    //Full time between letters + time between words
                    TimeForCompletingCurrentText += lScaledTimeBtwCharacters * lWordTexts[i].Length + lScaledTimeBtwWords;
                }

                for (int i = 0; i < lWordTexts.Length; i++)
                {
                    for (int j = 0; j < lWordTexts[i].Length; j++)
                    {
                        maxVisibleCharacters++;

                        yield return new WaitForSecondsRealtime(lScaledTimeBtwCharacters);//Time Btw Letters
                        TimeForCompletingCurrentText -= lScaledTimeBtwCharacters;
                    }

                    maxVisibleCharacters++;//For The Space
                    yield return new WaitForSecondsRealtime(lScaledTimeBtwWords);//Time Btw words
                    TimeForCompletingCurrentText -= lScaledTimeBtwWords;
                }

                yield return null;

                TimeForCompletingCurrentText = 0;
                readingRoutine = null;

                if (writingSFX != null)
                    writingSFX.Stop();

                onDialogueEnd.Invoke();
            }
        }
    }
}