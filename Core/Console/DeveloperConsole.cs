using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using XiheFramework.Core.UI.UIEntity;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Console {
    public class DeveloperConsoleUIOverlay : UIOverlayEntityBase {
        public GameObject consolePanel;
        public TMP_InputField inputField;
        public TextMeshProUGUI outputText;
        public UnityEngine.UI.Button clearButton;
        public UnityEngine.UI.Button helpButton;

        private bool m_IsOpen;
        private List<string> m_Logs = new List<string>();

        public override void OnInitCallback() {
            base.OnInitCallback();

            consolePanel.SetActive(m_IsOpen);

            clearButton.onClick.AddListener(() => {
                m_Logs.Clear();
                outputText.text = string.Empty;
            });

            helpButton.onClick.AddListener(() => {
                var availableCommandNames = Game.Command.GetAvailableCommandNames();
                m_Logs.Add("Available Commands:");
                m_Logs.AddRange(availableCommandNames);
                outputText.text = string.Join("\n", m_Logs);
            });
        }
    }
}