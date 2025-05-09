// -----------------------------------
// XiheFramework
// Author: sky_haihai, yifeng
// -----------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Blackboard;
using XiheFramework.Core.Entity;
using XiheFramework.Core.Event;
using XiheFramework.Core.Resource;
using XiheFramework.Core.Utility.DataStructure;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Base {
    /// <summary>
    /// Game manager for XiheFramework
    /// Execution order should be the earliest
    /// </summary>
    [DefaultExecutionOrder(300)]
    public class GameManager : MonoBehaviour {
        public string gameName = "My Game";

        // essential game modules
        public BlackboardModuleBase blackboardModule;
        public EntityModuleBase entityModule;
        public EventModule eventModule;
        public ResourceModule resourceModule;

        /// <summary>
        /// drag all custom game modules here
        /// </summary>
        public List<GameModuleBase> customGameModules;

        private readonly Dictionary<Type, GameModuleBase> m_PresetGameModules = new();
        private readonly Dictionary<Type, GameModuleBase> m_AliveGameModules = new();
        private readonly Queue<GameModuleBase> m_RegisterGameModulesQueue = new();
        private readonly Dictionary<Type, Action> m_GameModuleOnInstantiatedCallbacks = new();
        private readonly Dictionary<Type, int> m_AliveGameModuleUpdateTimers = new();
        private readonly Dictionary<Type, int> m_AliveGameModuleFixedUpdateTimers = new();
        private readonly Dictionary<Type, int> m_AliveGameModuleLateUpdateTimers = new();
        private readonly MultiDictionary<int, Type> m_AliveGameModulePriorityBuckets = new();

        #region Lifecycle

        private void Awake() {
            //cache all core game modules
            m_PresetGameModules[blackboardModule.GetType()] = blackboardModule;
            m_PresetGameModules[entityModule.GetType()] = entityModule;
            m_PresetGameModules[eventModule.GetType()] = eventModule;
            m_PresetGameModules[resourceModule.GetType()] = resourceModule;

            //cache all pre-set game module prefabs
            customGameModules.ForEach(module => m_PresetGameModules[module.GetType()] = module);
            Debug.LogFormat("XiheFramework Initialized");
            Game.Manager = this;
        }

        private void Start() {
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            foreach (var aliveGameModule in m_AliveGameModules) {
                if (m_AliveGameModuleUpdateTimers[aliveGameModule.Key] > 0) {
                    m_AliveGameModuleUpdateTimers[aliveGameModule.Key] -= 1;
                    continue;
                }

                aliveGameModule.Value.OnUpdateInternal();
                m_AliveGameModuleUpdateTimers[aliveGameModule.Key] = aliveGameModule.Value.updateInterval;
            }
        }

        private void FixedUpdate() { }

        private void LateUpdate() {
            //register game modules
            while (m_RegisterGameModulesQueue.Count > 0) {
                var gameModuleBase = m_RegisterGameModulesQueue.Dequeue();
                m_AliveGameModules[gameModuleBase.GetType()] = gameModuleBase;
                gameModuleBase.OnInstantiatedInternal(m_GameModuleOnInstantiatedCallbacks[gameModuleBase.GetType()]);
                m_AliveGameModulePriorityBuckets[gameModuleBase.Priority].Add(gameModuleBase.GetType());
            }
        }

        #endregion

        #region Public Methods

        public static void InstantiatePresetGameModule<T>(Action onInstantiated) where T : GameModuleBase {
            var gameModuleType = typeof(T);

            if (!Instance.m_PresetGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} does not exist, please create a prefab and drag it into XiheFramework Gameobject");
                return;
            }

            if (Instance.m_AliveGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER]Component: {gameModuleType} has already been registered");
                return;
            }

            var gameModule = Instantiate(Instance.m_PresetGameModules[gameModuleType]);
            if (gameModule == null) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} failed to instantiate");
                return;
            }

            Instance.m_AliveGameModules[gameModuleType] = gameModule;
            gameModule.OnInstantiatedInternal(onInstantiated);
        }

        public static void InstantiatePresetGameModuleAsync<T>(Action onInstantiated) where T : GameModuleBase {
            var gameModuleType = typeof(T);

            if (!Instance.m_PresetGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} does not exist, please create a prefab and drag it into XiheFramework Gameobject");
                return;
            }

            if (Instance.m_AliveGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER]Component: {gameModuleType} has already been registered");
                return;
            }

            var gameModule = Instantiate(Instance.m_PresetGameModules[gameModuleType]);
            if (gameModule == null) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} failed to instantiate");
                return;
            }

            Instance.m_RegisterGameModulesQueue.Enqueue(gameModule);
            Instance.m_GameModuleOnInstantiatedCallbacks[gameModuleType] = onInstantiated;
        }

        /// <summary>
        /// Get GameModule
        /// Should use Game.ModuleName instead
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetModule<T>() where T : GameModuleBase {
            var t = typeof(T);
            if (Instance == null) return null;

            if (Instance.m_AliveGameModules.TryGetValue(t, out var value)) return (T)value;

            Debug.LogErrorFormat("[GAME MANAGER]Component: {0} does not exist", t.Name);
            return null;
        }

        public static void DestroyFramework() {
            foreach (var component in Instance.m_AliveGameModules.Values) component.OnDestroyedInternal();
            Instance.m_AliveGameModules.Clear();
            Instance.m_RegisterGameModulesQueue.Clear();
        }

        public static string GameName => Instance.gameName;

        #endregion

        #region Singleton

        private static GameManager m_Instance;

        private static GameManager Instance {
            get {
                if (m_Instance == null) m_Instance = FindObjectOfType<GameManager>();

                return m_Instance;
            }
        }

        #endregion
    }
}