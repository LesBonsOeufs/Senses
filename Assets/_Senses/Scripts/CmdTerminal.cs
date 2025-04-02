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
            directoryPrefixTmp.text = "C:\\TestTest>";
            inputField.onSubmit.AddListener(InputField_OnSubmit);
        }

        public void InputField_OnSubmit(string text)
        {
            if (inputField.text == "")
                return;

            inputField.text = "";
            Instantiate(writtenLinePrefab, transform).text = $"{directoryPrefixTmp.text}{text}";
            directoryPrefixTmp.transform.SetAsLastSibling();

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