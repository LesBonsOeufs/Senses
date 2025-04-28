using NaughtyAttributes;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    [RequireComponent(typeof(RectTransform))]
    public class CmdTerminal : MonoBehaviour
    {
        [SerializeField] private WindowPullBase windowPuller;
        [SerializeField, ReadOnly] private RectTransform containerPanel;
        [SerializeField] private TextMeshProUGUI directoryTmp;
        [SerializeField] private TextMeshProUGUI writtenLinePrefab;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            InsertInteraction(null, new Interpreter.SResult
            {
                output = NodeManager.Instance.Current.AccessText,
                directory = NodeManager.Instance.Current.Directory,
                windowFromDirectory = NodeManager.Instance.Current.WindowFromDirectory
            });
        }

        public void InsertInteraction(string input, Interpreter.SResult result)
        {
            TextMeshProUGUI lInputLine = null;
            if (input != null)
            {
                lInputLine = Instantiate(writtenLinePrefab, transform);
                string lSimplifiedDirectoryText = Regex.Replace(directoryTmp.text, "<.*?>", string.Empty);
                lInputLine.text = $"{lSimplifiedDirectoryText}{input}";
            }
            
            TextMeshProUGUI lOutputLine = Instantiate(writtenLinePrefab, transform);
            lOutputLine.text = result.output;

            // Could be moved elsewhere
            if (result.type == NodeManager.EResult.POSITIONAL)
            {
                if (result.directory != null)
                    directoryTmp.text = $"X:\\{result.directory}>";

                if (result.windowFromDirectory != null)
                {
                    if (result.windowFromDirectory != windowPuller.windowPrefab)
                    {
                        windowPuller.windowPrefab = result.windowFromDirectory;
                        windowPuller.WindowOut();
                    }

                    directoryTmp.text = $"<color=yellow>{directoryTmp.text}</color>";
                }
                else
                {
                    windowPuller.windowPrefab = null;
                    windowPuller.WindowOut();
                }
            }
            //

            directoryTmp.transform.SetAsLastSibling();
            Interpreter.Instance.transform.SetAsLastSibling();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            float lInputHeight = lInputLine ? lInputLine.rectTransform.rect.height : 0f;
            float lOutputHeight = lOutputLine.rectTransform.rect.height;

            float lHardHeightLimit = containerPanel.rect.height * 3f;
            float lSoftHeightLimit = containerPanel.rect.height * 0.87f;

            float lLostHeight = 0f;
            float lRectTransformPredictiveHeight() => rectTransform.rect.height + lInputHeight + lOutputHeight - lLostHeight;

            if (lRectTransformPredictiveHeight() > lSoftHeightLimit)
            {
                float lHeightToAddForSoftLimit = Mathf.Max(lSoftHeightLimit - rectTransform.rect.height, 0f);

                rectTransform.anchoredPosition += 
                    Vector2.up * (lOutputHeight + lInputHeight - lHeightToAddForSoftLimit);
            }

            int lChildIndex = 0;

            //Test with lostHeight is required as Destroy's execution will not happen during the while loop
            //All lines that make terminal reach hardlimit are assumed to not be visible (borders between hardlimit & softlimit)
            while (lRectTransformPredictiveHeight() > lHardHeightLimit)
            {
                GameObject lFirstLine = transform.GetChild(lChildIndex++).gameObject;
                lLostHeight += lFirstLine.GetComponent<RectTransform>().rect.height;
                Destroy(lFirstLine);
            }

            rectTransform.anchoredPosition -= Vector2.up * lLostHeight;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private void OnValidate()
        {
            if (transform.parent == null)
                return;

            containerPanel = transform.parent.GetComponent<RectTransform>();
        }
    }
}