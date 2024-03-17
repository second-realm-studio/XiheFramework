using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XiheFramework.Combat.Base;
using XiheFramework.Entry;

namespace XiheFramework.Combat.Animation.TestRoom {
    public class PlayAnimation : MonoBehaviour {
        public TMP_InputField inputField;
        public TMP_Dropdown playOptionDropdown;
        public Button playBtn;
        public Slider speedSlider;
        public TextMeshProUGUI speedText;
        public TMP_InputField animationIntervalInputField;

        public CombatEntity owner;

        private Animation2DEntity m_Animation2DEntity;

        private void Start() {
            playBtn.onClick.AddListener(Play);
            speedSlider.value = XiheFramework.Entry.Game.LogicTime.GlobalTimeScale;
            speedSlider.onValueChanged.AddListener(ChangeSpeed);
            animationIntervalInputField.text = "2";
        }

        private void Update() {
            speedText.text = speedSlider.value.ToString("F2");
        }

        private void ChangeSpeed(float arg0) {
            XiheFramework.Entry.Game.LogicTime.SetGlobalTimeScalePermanent(arg0);
        }

        private void Play() {
            int interval = 2;
            try {
                interval = int.Parse(animationIntervalInputField.text);
            }
            catch (Exception) {
                interval = 2;
                animationIntervalInputField.text = "2";
            }

            if (m_Animation2DEntity != null) {
                Game.Animation2D.DestroyAnimation(m_Animation2DEntity);
            }

            var animationName = inputField.text;
            try {
                m_Animation2DEntity = Game.Animation2D.Create(owner, animationName);
                switch (playOptionDropdown.value) {
                    case 0:
                        m_Animation2DEntity.Play(EndBehaviour.Loop, interval);
                        break;
                    case 1:
                        m_Animation2DEntity.Play(EndBehaviour.Pause, interval);
                        break;
                    case 2:
                        m_Animation2DEntity.Play(EndBehaviour.Stop, interval);
                        break;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}