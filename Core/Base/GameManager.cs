using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Utility;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Base {
    /// <summary>
    /// Game manager for XiheFramework, work as a Singleton
    /// </summary>
    public class GameManager : Singleton<GameManager> {
        public static readonly string OnXiheFrameworkInitialized = "OnXiheFrameworkInitialized";
        public int frameRate = 60;
        public bool dontDestroyOnLoad = true;

        private readonly Dictionary<Type, GameModule> m_GameModules = new();

        private readonly Queue<GameModule> m_RegisterGameModulesQueue = new();

        private bool m_OnInitEventInvoked = false;

        private void Awake() {
            Application.targetFrameRate = frameRate;

            RegisterAllComponent();
            foreach (var component in m_GameModules.Values) component.Setup();
            Debug.LogFormat("XiheFramework Initialized");
        }

        private void Start() {
            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }

            //late start for all modules
            foreach (var component in m_GameModules.Values) component.OnLateStart();
        }

        private void Update() {
            if (m_OnInitEventInvoked) {
                return;
            }

            Game.Event.Invoke(OnXiheFrameworkInitialized);
            m_OnInitEventInvoked = true;
        }

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

        public static void ResetFramework() {
            foreach (var component in Instance.m_GameModules.Values) component.OnReset();
            Instance.m_GameModules.Clear();
            Instance.m_RegisterGameModulesQueue.Clear();
        }
    }
}