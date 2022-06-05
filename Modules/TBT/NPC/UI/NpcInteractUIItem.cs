using UnityEngine;
using UnityEngine.UI;

namespace XiheFramework {
    public class NpcInteractUIItem : MonoBehaviour {
        public Image iconImg;
        public Text eventNameTxt;

        public Button eventBtn;

        public string eventInternalName;

        private void Start() {
            eventBtn = GetComponent<Button>();
            eventBtn.onClick.AddListener(OnClick);
        }

        private void OnClick() {
            Game.Event.Invoke("OnFlowEventInvoked", null, eventInternalName);
            //close ui
            
        }

        public void Setup(Sprite icon, string eventDisplayName, string internalName) {
            iconImg.sprite = icon;
            eventNameTxt.text = eventDisplayName;
            this.eventInternalName = internalName;
        }
    }
}