using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using XiheFramework.Utility;

namespace XiheFramework.Modules.Base {
    /// <summary>
    /// Game manager for XiheFramework, work as a Singleton
    /// </summary>
    public class GameManager : Singleton<GameManager> {
        private const int FrameAtSceneId = 0;
        public int frameRate = 60;
        public float globalSpeed = 1f;

        public float logoDisplayTime;

        public string retryScene;
        public string restartScene;

        private readonly Dictionary<Type, GameModule> m_GameModules = new();

        private readonly Queue<GameModule> m_RegisterGameModulesQueue = new();

        private void Awake() {
            Application.targetFrameRate = frameRate;

            RegisterAllComponent();
            foreach (var component in m_GameModules.Values) component.Setup();

            Debug.LogFormat("XiheFramework Initialized");
        }

        private void Start() {
            DontDestroyOnLoad(gameObject);

            //play xihe logo
            if (logoDisplayTime > 0) {
                Game.Blackboard.SetData("System.LogoDisplayTime", logoDisplayTime);
                Game.FlowEvent.StartEvent("System.DisplayXiheLogo");
            }
        }

        private void Update() {
            Time.timeScale = globalSpeed;
        }

        private void OnEnable() { }

        private void RegisterAllComponent() {
            while (m_RegisterGameModulesQueue.Count > 0) {
                var module = m_RegisterGameModulesQueue.Dequeue();

                Instance.m_GameModules.Add(module.GetType(), module);
            }
        }

        /// <summary>
        /// Register GameModule to let the framework recognize it
        /// </summary>
        /// <param name="component"></param>
        public static void RegisterComponent(GameModule component) {
            if (component == null) {
                Debug.LogErrorFormat("[GAME MANAGER]Registering a null component");
                return;
            }

            if (Instance.m_GameModules.ContainsKey(component.GetType())) {
                Debug.LogErrorFormat("[GAME MANAGER]Component: {0} has already existed", component.GetType().Name);
                return;
            }

            //Instance.m_GameComponents.Add(component.GetType(), component);
            Instance.m_RegisterGameModulesQueue.Enqueue(component);
        }

        /// <summary>
        /// Get GameModule
        /// Should use Game.ModuleName instead
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetModule<T>() where T : GameModule {
            var t = typeof(T);
            if (Instance == null) return null;

            if (Instance.m_GameModules.TryGetValue(t, out var value)) return (T)value;

            Debug.LogErrorFormat("[GAME MANAGER]Component: {0} does not exist", t.Name);
            return null;
        }

        /// <summary>
        /// Shut down the game with ShutDownType 
        /// </summary>
        /// <param name="shutDownType"></param>
        public static void ShutDown(ShutDownType shutDownType) {
            for (var i = 0; i < Instance.m_GameModules.Count; i++) Instance.m_GameModules.ElementAt(i).Value.ShutDown(shutDownType);

            //Instance.m_GameComponents.Clear();

            switch (shutDownType) {
                case ShutDownType.None:
                    break;
                case ShutDownType.Retry:
                    SceneManager.LoadScene(Instance.retryScene);
                    break;
                case ShutDownType.Restart:
                    SceneManager.LoadScene(Instance.restartScene);
                    break;
                case ShutDownType.Quit:
                    Application.Quit();
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#endif
                    break;
                default:
                    break;
            }
        }
    }
}