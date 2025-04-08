using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

            float lHardHeightLimit = containerPanel.rect.height * 1.5f;
            float lSoftHeightLimit = containerPanel.rect.height * 0.7f;

            while (rectTransform.rect.height > lHardHeightLimit)
            {
                GameObject lFirstLine = transform.GetChild(0).gameObject;
                float lLostHeight = lFirstLine.GetComponent<RectTransform>().rect.height;
                Destroy(lFirstLine);
                rectTransform.anchoredPosition -= Vector2.up * lLostHeight;
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            if (rectTransform.rect.height > lSoftHeightLimit)
            {
                float lInputHeight = lInputLine.rectTransform.rect.height;
                float lOutputHeight = lOutputLine.rectTransform.rect.height;

                float lExcessiveHeight = rectTransform.rect.height - lOutputHeight <= lSoftHeightLimit ?
                        lInputHeight : lInputHeight + lOutputHeight;

                rectTransform.anchoredPosition += Vector2.up * lExcessiveHeight;
            }
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