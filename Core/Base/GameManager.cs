// -----------------------------------
// XiheFramework
// Author: sky_haihai, yifeng
// -----------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Core.Audio;
using XiheFramework.Core.Blackboard;
using XiheFramework.Core.Config;
using XiheFramework.Core.Console;
using XiheFramework.Core.Entity;
using XiheFramework.Core.Event;
using XiheFramework.Core.FSM;
using XiheFramework.Core.LogicTime;
using XiheFramework.Core.Resource;
using XiheFramework.Core.Scene;
// using XiheFramework.Core.Serialization;
using XiheFramework.Core.UI;
using XiheFramework.Core.Utility.DataStructure;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Base {
    /// <summary>
    /// Game manager for XiheFramework
    /// </summary>
    [DefaultExecutionOrder(300)]
    public class GameManager : MonoBehaviour {
        public string gameName = "My Game";

        #region Core Game Modules

        public AudioModuleBase audioModule;
        public BlackboardModuleBase blackboardModule;
        public ConfigModule configModule;
        public ConsoleModule consoleModule;
        public EntityModuleBase entityModule;
        public EventModule eventModule;
        public StateMachineModule fsmModule;
        public LogicTimeModule logicTimeModule;
        public ResourceModule resourceModule;
        public SceneModule sceneModule;
        public UIModule uiModule;
        // public SerializationModuleBase serializationModule;

        #endregion

        [Tooltip("drag all custom game module prefabs here")]
        public List<GameModuleBase> customGameModules;

        private readonly Dictionary<Type, GameModuleBase> m_PresetGameModules = new();
        private readonly Dictionary<Type, GameModuleBase> m_AliveGameModules = new();
        private readonly Queue<GameModuleBase> m_RegisterGameModulesQueue = new();
        private readonly Dictionary<Type, Action> m_GameModuleOnInstantiatedCallbacks = new();
        private readonly Dictionary<Type, int> m_AliveGameModuleUpdateTimers = new();
        private readonly Dictionary<Type, int> m_AliveGameModuleFixedUpdateTimers = new();
        private readonly Dictionary<Type, int> m_AliveGameModuleLateUpdateTimers = new();
        private MultiDictionary<int, Type> m_AliveGameModulePriorityBuckets = new();

        private Transform m_GameModuleRoot;

        #region Lifecycle

        private void Awake() {
            //create root
            m_GameModuleRoot = new GameObject("GameModules(Instantiated)").transform;
            m_GameModuleRoot.SetParent(Instance.transform);
            //cache all core game modules
            if (audioModule != null) m_PresetGameModules[audioModule.GetType()] = audioModule;
            if (blackboardModule != null) m_PresetGameModules[blackboardModule.GetType()] = blackboardModule;
            if (configModule != null) m_PresetGameModules[configModule.GetType()] = configModule;
            if (consoleModule != null) m_PresetGameModules[consoleModule.GetType()] = consoleModule;
            if (entityModule != null) m_PresetGameModules[entityModule.GetType()] = entityModule;
            if (eventModule != null) m_PresetGameModules[eventModule.GetType()] = eventModule;
            if (fsmModule != null) m_PresetGameModules[fsmModule.GetType()] = fsmModule;
            if (logicTimeModule != null) m_PresetGameModules[logicTimeModule.GetType()] = logicTimeModule;
            if (resourceModule != null) m_PresetGameModules[resourceModule.GetType()] = resourceModule;
            if (sceneModule != null) m_PresetGameModules[sceneModule.GetType()] = sceneModule;
            if (uiModule != null) m_PresetGameModules[uiModule.GetType()] = uiModule;
            // if (serializationModule != null) m_PresetGameModules[serializationModule.GetType()] = serializationModule;

            //instantiate all core game modules
            InstantiateCoreGameModules();

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

        private void FixedUpdate() {
            foreach (var aliveGameModule in m_AliveGameModules) {
                if (m_AliveGameModuleFixedUpdateTimers[aliveGameModule.Key] > 0) {
                    m_AliveGameModuleFixedUpdateTimers[aliveGameModule.Key] -= 1;
                    continue;
                }

                aliveGameModule.Value.OnFixedUpdateInternal();
                m_AliveGameModuleFixedUpdateTimers[aliveGameModule.Key] = aliveGameModule.Value.fixedUpdateInterval;
            }
        }

        private void LateUpdate() {
            foreach (var aliveGameModule in m_AliveGameModules) {
                if (m_AliveGameModuleLateUpdateTimers[aliveGameModule.Key] > 0) {
                    m_AliveGameModuleLateUpdateTimers[aliveGameModule.Key] -= 1;
                    continue;
                }

                aliveGameModule.Value.OnLateUpdateInternal();
                m_AliveGameModuleLateUpdateTimers[aliveGameModule.Key] = aliveGameModule.Value.lateUpdateInterval;
            }

            //register game modules
            ProcessGameModuleRegistrationQueue();
        }

        #endregion

        #region Public Methods

        public static void InstantiatePresetGameModule(Type gameModuleType, Action onInstantiated = null) {
            if (!Instance.m_PresetGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} does not exist, please create a prefab and drag it into XiheFramework Gameobject");
                return;
            }

            if (Instance.m_AliveGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER]Component: {gameModuleType} has already been registered");
                return;
            }

            var gameModule = Instantiate(Instance.m_PresetGameModules[gameModuleType], Instance.m_GameModuleRoot, true);
            if (gameModule == null) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} failed to instantiate");
                return;
            }

            Instance.m_AliveGameModules[gameModuleType] = gameModule;
            gameModule.OnInstantiatedInternal(onInstantiated);

            //timer
            Instance.m_AliveGameModuleUpdateTimers[gameModuleType] = gameModule.updateInterval;
            Instance.m_AliveGameModuleFixedUpdateTimers[gameModuleType] = gameModule.fixedUpdateInterval;
            Instance.m_AliveGameModuleLateUpdateTimers[gameModuleType] = gameModule.lateUpdateInterval;

            Instance.m_AliveGameModules[gameModule.GetType()] = gameModule;
            Instance.m_AliveGameModulePriorityBuckets.Add(gameModule.Priority, gameModule.GetType());
        }

        public static void InstantiatePresetGameModuleAsync(Type gameModuleType, Action onInstantiated) {
            if (!Instance.m_PresetGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} does not exist, please create a prefab and drag it into XiheFramework Gameobject");
                return;
            }

            if (Instance.m_AliveGameModules.ContainsKey(gameModuleType)) {
                Debug.LogError($"[GAME MANAGER]Component: {gameModuleType} has already been registered");
                return;
            }

            var gameModule = Instantiate(Instance.m_PresetGameModules[gameModuleType], Instance.m_GameModuleRoot, true);
            if (gameModule == null) {
                Debug.LogError($"[GAME MANAGER] GameModule: {gameModuleType.Name} failed to instantiate");
                return;
            }

            Instance.m_RegisterGameModulesQueue.Enqueue(gameModule);
            Instance.m_GameModuleOnInstantiatedCallbacks[gameModuleType] = onInstantiated;

            //timer
            Instance.m_AliveGameModuleUpdateTimers[gameModuleType] = gameModule.updateInterval;
            Instance.m_AliveGameModuleFixedUpdateTimers[gameModuleType] = gameModule.fixedUpdateInterval;
            Instance.m_AliveGameModuleLateUpdateTimers[gameModuleType] = gameModule.lateUpdateInterval;

            Instance.m_AliveGameModules[gameModule.GetType()] = gameModule;
            Instance.m_AliveGameModulePriorityBuckets.Add(gameModule.Priority, gameModule.GetType());
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

        #region Private Methods

        private void InstantiateCoreGameModules() {
            var tempDic = m_PresetGameModules.OrderBy(x => x.Value.Priority).ToDictionary(x => x.Key, x => x.Value);

            foreach (var gameModule in tempDic) {
                InstantiatePresetGameModule(gameModule.Key);
            }
        }

        private void ProcessGameModuleRegistrationQueue() {
            while (m_RegisterGameModulesQueue.Count > 0) {
                var gameModuleBase = m_RegisterGameModulesQueue.Dequeue();
                gameModuleBase.OnInstantiatedInternal(m_GameModuleOnInstantiatedCallbacks[gameModuleBase.GetType()]);
            }

            //sort by priority
            m_AliveGameModulePriorityBuckets = m_AliveGameModulePriorityBuckets
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

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