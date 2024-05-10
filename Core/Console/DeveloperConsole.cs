using System.Collections.Generic;
using DevConsole;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Console {
    public class DeveloperConsole : MonoBehaviour {
        public GameObject consolePanel;
        public TMP_InputField inputField;
        public TextMeshProUGUI outputText;
        public Button clearButton;

        private bool m_IsOpen;
        private List<string> m_Logs = new List<string>();

        private void Start() {
            DontDestroyOnLoad(gameObject);

            consolePanel.SetActive(m_IsOpen);

            clearButton.onClick.AddListener(() => {
                m_Logs.Clear();
                outputText.text = string.Empty;
            });
        }

        private void Update() {
            if (Game.Input.GetButtonDown("OpenDevConsole")) {
                Debug.Log("Open Dev Console");
                m_IsOpen = !m_IsOpen;
                consolePanel.SetActive(m_IsOpen);
                Game.Input.controllers.maps.SetMapsEnabled(!m_IsOpen, 0);
                Game.Input.controllers.maps.SetMapsEnabled(m_IsOpen, 1);
                Game.LogicTime.SetGlobalTimeScalePermanent(m_IsOpen?0f:1f);
            }

            if (m_IsOpen && Game.Input.GetButtonDown("ExecuteCommand")) {
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                var succeed = CommandFactory.ExecuteCommand(inputField.text);
                if (succeed) {
                    m_Logs.Add(inputField.text);
                }
                else {
                    m_Logs.Add($"Invalid Command: {inputField.text}");
                }

                outputText.text = string.Join("\n", m_Logs);

                inputField.text = string.Empty;
            }
        }
    }
}
