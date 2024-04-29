using System;
#if USE_TMP
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;
using XiheFramework.Combat.Base;
using XiheFramework.Core;

namespace XiheFramework.Combat.Animation.TestRoom {
    public class PlayAnimation : MonoBehaviour {
#if USE_TMP
        public TMP_InputField inputField;
        public TMP_Dropdown playOptionDropdown;
        public TextMeshProUGUI speedText;
        public TMP_InputField animationIntervalInputField;
#endif
        public Button playBtn;
        public Slider speedSlider;


        public CombatEntity owner;

        private Animation2DEntity m_Animation2DEntity;

        private void Start() {
            playBtn.onClick.AddListener(Play);
            speedSlider.value = GameCore.LogicTime.GlobalTimeScale;
            speedSlider.onValueChanged.AddListener(ChangeSpeed);
#if USE_TMP
            animationIntervalInputField.text = "2";
#endif
        }

        private void Update() {
#if USE_TMP
            speedText.text = speedSlider.value.ToString("F2");
#endif
        }

        private void ChangeSpeed(float arg0) {
            GameCore.LogicTime.SetGlobalTimeScalePermanent(arg0);
        }

        private void Play() {
            int interval = 2;
            try {
#if USE_TMP
                interval = int.Parse(animationIntervalInputField.text);
#endif
            }
            catch (Exception) {
                interval = 2;
#if USE_TMP
                animationIntervalInputField.text = "2";
#endif
            }

            if (m_Animation2DEntity != null) {
                GameCombat.Animation2D.DestroyAnimation(m_Animation2DEntity);
            }

#if USE_TMP
            var animationName = inputField.text;
#endif
            try {
#if USE_TMP
                m_Animation2DEntity = Game.Animation2D.Create(owner, animationName);
#endif

#if USE_TMP
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
#else
                m_Animation2DEntity.Play(EndBehaviour.Loop, interval);
#endif
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}