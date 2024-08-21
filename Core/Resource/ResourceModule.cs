using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif
using UnityEngine.SocialPlatforms;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;
using Object = UnityEngine.Object;

namespace XiheFramework.Core.Resource {
    /// <summary>
    /// use Unity Addressable to load assets
    /// </summary>
    public class ResourceModule : GameModule {
        private readonly Dictionary<string, Object> m_CachedAssets = new Dictionary<string, Object>();

        public override void Setup() {
#if USE_ADDRESSABLE
            Addressables.InitializeAsync(true);
#else
            Debug.LogError("Please import Addressable Package and define USE_ADDRESSABLE in your project settings: Player->Other Settings->Scripting Define Symbols");
#endif
        }

        #region Sync

        public T InstantiateAsset<T>(string address) where T : Object {
            var asset = LoadAsset<T>(address);
            if (asset == null) {
                return null;
            }

            return Instantiate(asset);
        }

        public T LoadAsset<T>(string address) where T : Object {
#if USE_ADDRESSABLE
            //cached resource
            if (m_CachedAssets.ContainsKey(address)) {
                var res = m_CachedAssets[address] as T;
                if (res == null) {
                    Debug.LogError("LoadAsync failed: " + address + " exists but is not a " + typeof(T).Name);
                    return null;
                }

                return res;
            }

            try {
                AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(address);
                return op.WaitForCompletion();
            }
            catch (Exception e) {
                Debug.LogError(e);
                return null;
            }
#else
            return null;
#endif
        }

        #endregion


        #region Async

        public void InstantiateAssetAsync<T>(string address, Action<T> onInstantiated) where T : Object {
            LoadAssetAsync<T>(address, o => {
                var go = Instantiate(o);
                onInstantiated?.Invoke(go);
            });
        }

        public void LoadAssetAsync<T>(string address, Action<T> onLoaded) where T : Object {
            StartCoroutine(LoadAssetAsyncCoroutine<T>(address, onLoaded));
        }

        public IEnumerator LoadAssetAsyncCoroutine<T>(string address, Action<T> onLoaded) where T : Object {
#if USE_ADDRESSABLE
            var alreadyCached = m_CachedAssets.ContainsKey(address) && m_CachedAssets[address] != null && m_CachedAssets[address] is T;
            if (alreadyCached) {
                onLoaded?.Invoke((T)m_CachedAssets[address]);
            }
            else {
                if (m_CachedAssets.ContainsKey(address)) {
                    m_CachedAssets.Remove(address);
                }

                AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(address);
                yield return op;

                if (op.Status == AsyncOperationStatus.Succeeded) {
                    m_CachedAssets.Add(address, op.Result);
                }
                else {
                    Debug.LogError("[RESOURCE]LoadAsync failed: " + address + ". Msg:" + op.OperationException);
                }

                onLoaded?.Invoke(op.Result);

                Addressables.Release(op);
            }
#else
            Debug.LogError("Please import Addressable Package and define USE_ADDRESSABLE in your project settings: Player->Other Settings->Scripting Define Symbols");
            yield break;
#endif
        }

        public IEnumerator LoadAssetsAsyncCoroutine(IEnumerable<string> labels, Action<float> onProgress, Action<IEnumerable<Object>> onLoaded) {
#if USE_ADDRESSABLE
            var locationOpHandle = Addressables.LoadResourceLocationsAsync(labels, Addressables.MergeMode.Intersection);
            yield return locationOpHandle;

            if (locationOpHandle.Status == AsyncOperationStatus.Failed) {
                Debug.LogError("[RESOURCE]Load Locations failed. Msg:" + locationOpHandle.OperationException);
                yield break;
            }

            var results = new List<Object>();

            foreach (var location in locationOpHandle.Result) {
                if (m_CachedAssets.ContainsKey(location.PrimaryKey)) {
                    results.Add(m_CachedAssets[location.PrimaryKey]);
                    continue;
                }

                var address = location.PrimaryKey;
                var assetOpHandle = Addressables.LoadAssetAsync<Object>(address);
                yield return assetOpHandle;

                if (assetOpHandle.Status == AsyncOperationStatus.Succeeded) {
                    if (!m_CachedAssets.ContainsKey(address)) {
                        m_CachedAssets.Add(address, assetOpHandle.Result);
                    }

                    results.Add(assetOpHandle.Result);
                }
                else {
                    Debug.LogError("[RESOURCE]LoadAsync failed: " + location.PrimaryKey + ". Msg:" + assetOpHandle.OperationException);
                }

                onProgress?.Invoke(results.Count / (float)locationOpHandle.Result.Count);
            }

            onLoaded?.Invoke(results);
#else
            Debug.LogError("Please import Addressable Package and define USE_ADDRESSABLE in your project settings: Player->Other Settings->Scripting Define Symbols");
            yield break;
#endif
        }

        #endregion

        public override void OnReset() {
#if USE_ADDRESSABLE
            foreach (var asset in m_CachedAssets) {
                Addressables.Release(asset.Value);
            }

            m_CachedAssets.Clear();
#endif
        }

        protected override void Awake() {
            base.Awake();

            Game.Resource = this;
        }

        public void DebugPlayerEntity() {
            if (m_CachedAssets.ContainsKey("PlayerEntity_StandardPlayer")) {
                Debug.Log(m_CachedAssets["PlayerEntity_StandardPlayer"] == null ? " Player null" : "Player not null");
            }
        }
    }
}