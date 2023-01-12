using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class InputModule : GameModule {
        private readonly Dictionary<string, KeyCode> m_KeyActionBinds = new Dictionary<string, KeyCode>();

        private readonly Dictionary<string, TwoDirectionalKeySet> m_TwoAxisKeyBinds =
            new Dictionary<string, TwoDirectionalKeySet>();

        //public List<ActionKeyPair> actionKeyPairs;
        [SerializeField] private List<ActionKeyPair> bindingSetting = new List<ActionKeyPair>();

        [SerializeField] private List<TwoDirectionalKeySet> twoAxisKeySetting = new List<TwoDirectionalKeySet>();

        private Vector2 m_MouseDeltaPosition;
        private Vector2 m_LastFrameMousePosition;

        // private float m_WASDInputMultiplier = 0f;

        private Dictionary<string, float> m_TwoDirectionalMultipliers = new Dictionary<string, float>();
        // private float m_WASDInputAcceleration = 1f;

        // private float m_ArrowInputMultiplier = 0f;
        // private float m_ArrowInputAcceleration = 1f;

        public bool allowInput = true;

        protected override void Awake() {
            base.Awake();

            foreach (var pair in bindingSetting) {
                if (m_KeyActionBinds.ContainsKey(pair.action)) {
                    Debug.LogErrorFormat(
                        "[XIHE INPUT] Multiple keycodes are assigning to a same action, ignoring the action-key pair [{0},{1}]",
                        pair.action.ToString(), pair.key.ToString());
                }

                m_KeyActionBinds.Add(pair.action, pair.key);
            }

            foreach (var pair in twoAxisKeySetting) {
                if (m_TwoAxisKeyBinds.ContainsKey(pair.name)) {
                    Debug.LogErrorFormat(
                        "[XIHE INPUT] Multiple keycodes are assigning to a same twoAxisBinding, ignoring [{0},{1},{2},{3},{4}]",
                        pair.name, pair.forward.ToString(), pair.backward.ToString(), pair.left.ToString(),
                        pair.right.ToString());
                }

                m_TwoAxisKeyBinds.Add(pair.name, pair);
            }
        }

        // public KeyCode GetKeycode(KeyActionTypes action) {
        //     return keyActionBinds[action];
        // }

        public void SetKeycode(string action, KeyCode keyCode) {
            if (m_KeyActionBinds.ContainsKey(action)) {
                m_KeyActionBinds[action] = keyCode;
            }
            else {
                m_KeyActionBinds.Add(action, keyCode);
            }
        }

        public KeyCode GetKeyCode(string keyActionTypes) {
            if (m_KeyActionBinds.ContainsKey(keyActionTypes)) return m_KeyActionBinds[keyActionTypes];

            Game.Log.LogErrorFormat("[XIHE INPUT]key action type is not assigned with a valid key code");
            return KeyCode.None;
        }

        public bool ContainsKeyAction(string keyActionTypes) {
            return m_KeyActionBinds.ContainsKey(keyActionTypes);
        }

        public bool GetKeyDown(string keyActionTypes) => allowInput && Input.GetKeyDown(GetKeyCode(keyActionTypes));

        public bool GetKey(string keyActionTypes) => allowInput && Input.GetKey(GetKeyCode(keyActionTypes));

        public bool GetKeyUp(string keyActionTypes) => allowInput && Input.GetKeyUp(GetKeyCode(keyActionTypes));

        public bool GetMouseUp(int mouseType) {
            var btnUp = mouseType switch {
                0 => Input.GetButtonUp("Submit"),
                1 => Input.GetButtonUp("Cancel"),
                _ => throw new ArgumentOutOfRangeException(nameof(mouseType), mouseType, null)
            };
            return allowInput && (Input.GetMouseButtonUp(mouseType) || btnUp);
        }

        public bool GetMouseDown(int mouseType) {
            var btnDown = mouseType switch {
                0 => Input.GetButtonDown("Submit"),
                1 => Input.GetButtonDown("Cancel"),
                _ => throw new ArgumentOutOfRangeException(nameof(mouseType), mouseType, null)
            };
            return allowInput && (Input.GetMouseButtonDown(mouseType) || btnDown);
        }

        public bool GetMouse(int mouseType) {
            var btn = mouseType switch {
                0 => Input.GetButton("Submit"),
                1 => Input.GetButton("Cancel"),
                _ => throw new ArgumentOutOfRangeException(nameof(mouseType), mouseType, null)
            };
            return allowInput && (Input.GetMouseButton(mouseType) || btn);
        }

        public Vector2 GetXZInput() => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        public Vector2 GetMouseXZInput() => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // public Vector2 GetWASDInput() {
        //     Vector2 input = Vector2.zero; //w,s,a,d
        //     bool holding = false;
        //     if (Input.GetKey(KeyCode.W)) {
        //         input.y += m_WASDInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (Input.GetKey(KeyCode.S)) {
        //         input.y -= m_WASDInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (Input.GetKey(KeyCode.A)) {
        //         input.x -= m_WASDInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (Input.GetKey(KeyCode.D)) {
        //         input.x += m_WASDInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (holding) {
        //         m_WASDInputMultiplier += Time.deltaTime * m_WASDInputAcceleration;
        //     }
        //     else {
        //         m_WASDInputMultiplier -= Time.deltaTime * m_WASDInputAcceleration;
        //     }
        //
        //     m_WASDInputMultiplier = Mathf.Clamp01(m_WASDInputMultiplier);
        //
        //     return input;
        // }


        public Vector2 GetTwoAxisInput(string setName, float acceleration = 1f) {
            if (!m_TwoAxisKeyBinds.ContainsKey(setName)) {
                return Vector2.zero;
            }

            var set = m_TwoAxisKeyBinds[setName];

            float mul = 0f;
            if (!m_TwoDirectionalMultipliers.ContainsKey(setName)) {
                m_TwoDirectionalMultipliers.Add(setName, 0f);
            }
            else {
                mul = m_TwoDirectionalMultipliers[setName];
            }

            Vector2 input = Vector2.zero; //w,s,a,d
            bool holding = false;
            if (Input.GetKey(set.forward)) {
                input.y += mul;
                holding = true;
            }

            if (Input.GetKey(set.backward)) {
                input.y -= mul;
                holding = true;
            }

            if (Input.GetKey(set.left)) {
                input.x -= mul;
                holding = true;
            }

            if (Input.GetKey(set.right)) {
                input.x += mul;
                holding = true;
            }

            if (holding) {
                mul += Time.deltaTime * acceleration;
            }
            else {
                mul -= Time.deltaTime * acceleration;
            }

            mul = Mathf.Clamp01(mul);
            m_TwoDirectionalMultipliers[setName] = mul;

            return input;
        }

        // public Vector2 GetArrowInput() {
        //     Vector2 input = Vector2.zero; //↑ ↓ ← →
        //     bool holding = false;
        //     if (Input.GetKey(KeyCode.UpArrow)) {
        //         input.y += m_ArrowInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (Input.GetKey(KeyCode.DownArrow)) {
        //         input.y -= m_ArrowInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (Input.GetKey(KeyCode.LeftArrow)) {
        //         input.x -= m_ArrowInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (Input.GetKey(KeyCode.RightArrow)) {
        //         input.x += m_ArrowInputMultiplier;
        //         holding = true;
        //     }
        //
        //     if (holding) {
        //         m_ArrowInputMultiplier += Time.deltaTime * m_ArrowInputAcceleration;
        //     }
        //     else {
        //         m_ArrowInputMultiplier -= Time.deltaTime * m_ArrowInputAcceleration;
        //     }
        //
        //     m_ArrowInputMultiplier = Mathf.Clamp01(m_ArrowInputMultiplier);
        //
        //     return input;
        // }

        public Vector2 GetMouseDeltaPosition() {
            return m_MouseDeltaPosition;
        }

        private void UpdateMouseDeltaPosition() {
            var currentFrameMousePosition = Input.mousePosition;
            m_MouseDeltaPosition = currentFrameMousePosition.ToVector2(V3ToV2Type.XY) - m_LastFrameMousePosition;
            m_LastFrameMousePosition = currentFrameMousePosition;
        }

        public void DisableInput() {
            allowInput = false;
        }

        public void EnableInput() {
            allowInput = true;
        }

        public bool AnyKey() {
            return Input.anyKey;
        }

        public bool AnyKeyDown() {
            return Input.anyKeyDown;
        }

        public override void Update() {
            UpdateMouseDeltaPosition();
        }

        public override void ShutDown(ShutDownType shutDownType) { }

        [Serializable]
        public struct ActionKeyPair {
            public string action;
            public KeyCode key;

            public ActionKeyPair(string action, KeyCode key) {
                this.action = action;
                this.key = key;
            }
        }

        [Serializable]
        public struct TwoDirectionalKeySet {
            public string name;
            public KeyCode forward;
            public KeyCode backward;
            public KeyCode left;
            public KeyCode right;
        }

        // [Serializable]
        // private class ActionKeyCodeDictionary : SerializableDictionary<KeyActionTypes, KeyCode> { }
    }
}