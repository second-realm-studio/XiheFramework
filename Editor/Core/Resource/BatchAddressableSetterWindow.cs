using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
#if USE_ADDRESSABLE
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;
using XiheFramework.Core.Entity;

namespace XiheFramework.Editor.Core.Entity {
    public class BatchAddressableSetterWindow : EditorWindow {
        [MenuItem("XiheFramework/Batch Addressable Address Setter")]
        public static void ShowWindow() {
            GetWindow(typeof(BatchAddressableSetterWindow));
        }

        void OnGUI() {
            EditorGUILayout.LabelField("Addressable Addresses Batch Modifier", EditorStyles.boldLabel);

            if (Application.isPlaying) {
                EditorGUILayout.HelpBox("Disabled During Play Mode", MessageType.Warning);
                return;
            }

            // get current selected path
            string fullPath = "";
            if (Selection.activeObject != null) {
                fullPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (string.IsNullOrEmpty(fullPath)) {
                    EditorGUILayout.HelpBox("Select a Prefab to Start!", MessageType.Warning);
                    return;
                }

                fullPath = Path.GetDirectoryName(fullPath);
                fullPath = fullPath?.Replace('\\', '/');
            }
            else {
                EditorGUILayout.HelpBox("Please Select a Prefab First!", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Full Path: ", fullPath);

            if (GUILayout.Button("Generate")) {
                if (string.IsNullOrEmpty(fullPath) || fullPath == "Nothing selected!") {
                    Debug.LogError("Full Path cannot be empty!");
                    return;
                }

                if (!AssetDatabase.IsValidFolder(fullPath)) {
                    Debug.LogError("Path does not exist!");
                    return;
                }

                // get all prefabs in the folder
                // string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { fullPath });
                List<string> guids = new List<string>();
                foreach (var go in Selection.gameObjects) {
                    var path = AssetDatabase.GetAssetPath(go);
                    guids.Add(AssetDatabase.AssetPathToGUID(path));
                }

                string[] assetPaths = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToArray();

                // get Addressable Asset Settings
#if USE_ADDRESSABLE
                AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null) {
                    Debug.LogWarning("Addressable Asset Settings not found! Go to: Window->Asset Management->Addressables->Groups, Create Addressable Asset Settings.");
                    return;
                }

                foreach (string assetPath in assetPaths) {
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);
                    GameEntity entity = AssetDatabase.LoadAssetAtPath<GameEntity>(assetPath);
                    if (entity == null) {
                        settings.RemoveAssetEntry(guid);
                        continue;
                    }

                    if (string.IsNullOrEmpty(entity.EntityName)) {
                        settings.RemoveAssetEntry(guid);
                        continue;
                    }

                    string groupName = entity.EntityGroupName;

                    //find group
                    AddressableAssetGroup group = settings.FindGroup(groupName);
                    if (group == null) {
                        group = settings.CreateGroup(groupName, false, false, true, null);
                    }

                    string assetName = groupName + "_" + entity.EntityName;

                    // actually set address
                    AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
                    entry.address = assetName;
                }


                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#else
                Debug.LogError("Please import Addressable Package and define USE_ADDRESSABLE in your project settings: Player->Other Settings->Scripting Define Symbols");
#endif
            }
        }
    }
}