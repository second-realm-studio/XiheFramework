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
        public readonly string onDefaultResourcesLoadedEvtName = "Resource.OnDefaultResourcesLoaded";
        private readonly Dictionary<string, Object> m_CachedAssets = new Dictionary<string, Object>();
        // private readonly Dictionary<string, AsyncOperationHandle> m_CachedAssetHandles = new Dictionary<string, AsyncOperationHandle>();

        public override void Setup() {
#if USE_ADDRESSABLE
            Addressables.InitializeAsync(true);
#else
            Debug.LogError("Please import Addressable Package and define USE_ADDRESSABLE in your project settings: Player->Other Settings->Scripting Define Symbols");
#endif
        }

        #region Sync

        public T InstantiateAsset<T>(string address) where T : Object {
            return InstantiateAsset<T>(address, Vector3.zero, Quaternion.identity);
        }

        public T InstantiateAsset<T>(string address, Vector3 localPosition, Quaternion localRotation) where T : Object {
            var asset = LoadAsset<T>(address);
            if (asset == null) {
                return null;
            }

            return Instantiate(asset, localPosition, localRotation);
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
                var result = op.WaitForCompletion();
                if (result == null) {
                    Debug.LogError("LoadAsync failed: " + address);
                    return null;
                }

                return result;
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

        public void InstantiateAssetAsync<T>(string address, Vector3 localPosition, Quaternion localRotation, Action<T> onInstantiated) where T : Object {
            LoadAssetAsync<T>(address, o => {
                var go = Instantiate(o, localPosition, localRotation);
                onInstantiated?.Invoke(go);
            });
        }

        public void InstantiateAssetAsync<T>(string address, Vector3 localPosition, Action<T> onInstantiated) where T : Object {
            InstantiateAssetAsync(address, localPosition, Quaternion.identity, onInstantiated);
        }

        public void InstantiateAssetAsync<T>(string address, Action<T> onInstantiated) where T : Object {
            InstantiateAssetAsync(address, Vector3.zero, Quaternion.identity, onInstantiated);
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

                // if (m_CachedAssetHandles.ContainsKey(address)) {
                //     m_CachedAssetHandles.Remove(address);
                // }

                AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(address);
                yield return op;

                if (op.Status == AsyncOperationStatus.Succeeded) {
                    m_CachedAssets.Add(address, op.Result);
                    onLoaded?.Invoke(op.Result);
                    // m_CachedAssetHandles.Add(address, op);
                }
                else {
                    Debug.LogError("[RESOURCE]LoadAsync failed: " + address + ". Msg:" + op.OperationException);
                }
            }
#else
            Debug.LogError("Please import Addressable Package and define USE_ADDRESSABLE in your project settings: Player->Other Settings->Scripting Define Symbols");
            yield break;
#endif
        }

        public IEnumerator LoadAssetsAsyncCoroutine(IEnumerable<string> labels, Action<float> onProgress = null, Action<string> onLoaded = null,
            Action<IEnumerable<Object>> onFinished = null) {
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
                        onLoaded?.Invoke(address);
                        if (enableDebug) {
                            Debug.Log("[RESOURCE] Asset loaded: " + address);
                        }
                    }

                    results.Add(assetOpHandle.Result);
                }
                else {
                    Debug.LogError("[RESOURCE]LoadAsync failed: " + location.PrimaryKey + ". Msg:" + assetOpHandle.OperationException);
                }

                onProgress?.Invoke(results.Count / (float)locationOpHandle.Result.Count);
            }

            onFinished?.Invoke(results);
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

            // foreach (var asset in m_CachedAssetHandles) {
            //     Addressables.Release(asset.Value);
            // }

            m_CachedAssets.Clear();
#endif
        }

        protected override void Awake() {
            base.Awake();

            Game.Resource = this;
        }
    }
}