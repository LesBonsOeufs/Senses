using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Root
{
    [RequireComponent(typeof(RectTransform))]
    public class CmdTerminal : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI directoryPrefixTmp;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI writtenLinePrefab;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            directoryPrefixTmp.text = "C:\\Test>";
            inputField.onSubmit.AddListener(InputField_OnSubmit);
        }

        public async void InputField_OnSubmit(string text)
        {
            if (inputField.text != "")
            {
                inputField.enabled = false;
                string lText = inputField.text;
                inputField.text = "";

                string lResult = await Interpreter.Instance.Execute(lText);
                inputField.enabled = true;

                Instantiate(writtenLinePrefab, transform).text = $"{directoryPrefixTmp.text}{lResult}";
                directoryPrefixTmp.transform.SetAsLastSibling();
            }

            inputField.ActivateInputField();
            inputField.Select();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }
    }
}