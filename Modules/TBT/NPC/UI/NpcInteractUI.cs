using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XiheFramework {
    public class NpcInteractUI : MonoBehaviour {
        public NpcInteractUIItem template;

        private GridLayoutGroup m_Grid;
        private List<string> m_EventNames = new List<string>();

        public RectTransform RectTransform { get; private set; }


        private void Start() {
            m_Grid = GetComponent<GridLayoutGroup>();
            RectTransform = GetComponent<RectTransform>();
        }

        public void UpdateFlowEventItems(string[] events) {
            if (events == null) {
                return;
            }

            foreach (var eventName in events) {
                var e = Game.FlowEvent.GetEvent(eventName);
                var go = Instantiate(template, transform);
                if (e is NpcEvent ne) {
                    go.Setup(ne.icon, ne.displayName, ne.eventName);
                }
                else {
                    go.Setup(null, e.eventName, e.eventName);
                }
            }

            UpdateSize();
        }

        private void Update() {
        }

        void UpdateSize() {
            m_Grid = GetComponent<GridLayoutGroup>();
            var padding = m_Grid.padding;
            var cellSize = m_Grid.cellSize;
            var spacing = m_Grid.spacing;

            RectTransform = GetComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(cellSize.x + padding.left + padding.right,
                (cellSize.y + spacing.y) * RectTransform.childCount + padding.top + padding.bottom - spacing.y);
        }
    }
}