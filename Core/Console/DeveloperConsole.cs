using System.Collections.Generic;
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
        public Button helpButton;

        private bool m_IsOpen;
        private List<string> m_Logs = new List<string>();

        private void Start() {
            DontDestroyOnLoad(gameObject);

            consolePanel.SetActive(m_IsOpen);

            clearButton.onClick.AddListener(() => {
                m_Logs.Clear();
                outputText.text = string.Empty;
            });

            helpButton.onClick.AddListener(() => {
                var commands = CommandFactory.PrintAllCommands();
                m_Logs.Add("Commands:");
                m_Logs.AddRange(commands);
                outputText.text = string.Join("\n", m_Logs);
            });
        }

        private void Update() {
            if (Game.SystemInput.GetButtonDown("OpenDevConsole")) {
                m_IsOpen = !m_IsOpen;
                Debug.Log("Dev Console: " + (m_IsOpen ? "Open" : "Close"));

                consolePanel.SetActive(m_IsOpen);
                Game.LogicTime.SetGlobalTimeScalePermanent(m_IsOpen ? 0f : 1f);
            }

            if (m_IsOpen && Game.SystemInput.GetButtonDown("ExecuteCommand")) {
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