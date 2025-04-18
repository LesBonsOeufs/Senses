using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Root
{
    [RequireComponent(typeof(RectTransform))]
    public class CmdTerminal : MonoBehaviour
    {
        [SerializeField, ReadOnly] private RectTransform containerPanel;
        [SerializeField] private TextMeshProUGUI directoryPrefixTmp;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI writtenLinePrefab;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            directoryPrefixTmp.text = "C:\\Users\\PC>";
            inputField.onSubmit.AddListener(InputField_OnSubmit);
        }

        public async void InputField_OnSubmit(string text)
        {
            if (inputField.text != "")
            {
                inputField.enabled = false;
                string lResult = await Interpreter.Instance.Execute(text);
                inputField.enabled = true;
                inputField.text = "";

                InsertInteraction(text, lResult);
            }

            inputField.ActivateInputField();
            inputField.Select();
        }

        private void InsertInteraction(string input, string output)
        {
            TextMeshProUGUI lInputLine = Instantiate(writtenLinePrefab, transform);
            lInputLine.text = $"{directoryPrefixTmp.text}{input}";
            TextMeshProUGUI lOutputLine = Instantiate(writtenLinePrefab, transform);
            lOutputLine.text = output;

            directoryPrefixTmp.transform.SetAsLastSibling();
            Interpreter.Instance.transform.SetAsLastSibling();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            float lInputHeight = lInputLine.rectTransform.rect.height;
            float lOutputHeight = lOutputLine.rectTransform.rect.height;

            float lHardHeightLimit = containerPanel.rect.height * 3f;
            float lSoftHeightLimit = containerPanel.rect.height * 0.87f;

            float lLostHeight = 0f;
            float lRectTransformPredictiveHeight() => rectTransform.rect.height + lInputHeight + lOutputHeight - lLostHeight;

            if (lRectTransformPredictiveHeight() > lSoftHeightLimit)
                rectTransform.anchoredPosition += Vector2.up * (lOutputHeight + lInputHeight);

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

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }

        private void OnValidate()
        {
            containerPanel = transform.parent.GetComponent<RectTransform>();
        }
    }
}