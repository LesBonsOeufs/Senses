using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Root
{
    public class TerminalInputter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private CmdTerminal to;

        private void Awake()
        {
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

                to.InsertInteraction(text, lResult);
            }

            inputField.ActivateInputField();
            inputField.Select();
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }
    }
}