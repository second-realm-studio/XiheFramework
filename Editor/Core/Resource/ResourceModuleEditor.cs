using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
#if USE_ADDRESSABLE
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif
using UnityEngine;
using XiheFramework.Core.Entity;
using XiheFramework.Core.Resource;

namespace XiheFramework.Editor.Core.Resource {
    [CustomEditor(typeof(ResourceModule))]
    public class ResourceModuleEditor : UnityEditor.Editor {
        private const string AddressableResourcesRoot = "Assets/AddressableResources/";

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (AssetDatabase.IsValidFolder(AddressableResourcesRoot)) {
                GUI.enabled = true;
                GUILayout.Label("Resource Folder: ");
                GUI.enabled = false;
                GUILayout.TextField(AddressableResourcesRoot);
                GUILayout.Space(2f);
                GUI.enabled = true;
            }
        }
    }
}