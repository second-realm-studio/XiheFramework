using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using XiheFramework.Modules.Base;
using Object = UnityEngine.Object;

/// <summary>
/// use Unity Addressable to load assets
/// </summary>
public class ResourceModule : GameModule {
    private readonly Dictionary<string, Object> m_CachedAssets = new Dictionary<string, Object>();

    internal override void Setup() {
        Addressables.InitializeAsync(true);
    }

    public void LoadAsync<T>(string objectName, Action<T> callback) where T : Object {
        if (m_CachedAssets.ContainsKey(objectName)) {
            callback?.Invoke(m_CachedAssets[objectName] as T);
        }
        else {
            AsyncOperationHandle<T> asyncOperation = Addressables.LoadAssetAsync<T>(objectName);
            asyncOperation.Completed += (op) => {
                if (!m_CachedAssets.ContainsKey(objectName))
                    m_CachedAssets.Add(objectName, op.Result);

                callback?.Invoke(op.Result);
            };
        }
    }
}