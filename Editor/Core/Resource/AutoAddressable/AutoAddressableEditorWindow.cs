#if USE_ADDRESSABLE
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace AutoAddressable {
    public class AutoAddressableEditorWindow : EditorWindow {
        private const string AddressableResourcesRoot = "Assets/AddressableResources/";
        private const string TemplatePath = "Assets/AutoAddressable/AddressWrapperTemplate.txt";

        private bool m_CombineAllLinkScripts;

        [MenuItem("XiheFramework/Auto Addressable Editor")]
        public static void ShowWindow() {
            GetWindow(typeof(AutoAddressableEditorWindow));
        }

        private void OnGUI() {
            if (Application.isPlaying) {
                EditorGUILayout.HelpBox("Disabled During Play Mode", MessageType.Warning);
                return;
            }

            if (!AssetDatabase.IsValidFolder(AddressableResourcesRoot)) {
                if (GUILayout.Button("Create Addressable Resources Folder")) {
                    AssetDatabase.CreateFolder("Assets", "AddressableResources");
                }

                return;
            }

            m_CombineAllLinkScripts = GUILayout.Toggle(m_CombineAllLinkScripts, "Combine Link Scripts");

            if (GUILayout.Button("Update Addressable/Generate Link C# Scripts")) {
                MarkAllAddressable();
            }
        }

        private void MarkAllAddressable() {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) {
                Debug.LogWarning("Addressable Asset Settings not found! Go to: Window->Asset Management->Addressables->Groups, Create Addressable Asset Settings.");
                return;
            }

            var addressArrays = new Dictionary<string, string[]>();
            var rootValidAddresses = MarkAddressablePerGroup(AddressableResourcesRoot, "Default Local Group", SearchOption.TopDirectoryOnly);
            addressArrays.Add("DefaultLocalGroup", rootValidAddresses);

            var subFolders = Directory.GetDirectories(AddressableResourcesRoot, "*", SearchOption.TopDirectoryOnly);
            foreach (var path in subFolders) {
                var groupName = new DirectoryInfo(path).Name;
                var validAddresses = MarkAddressablePerGroup(path, groupName, SearchOption.AllDirectories);
                if (!m_CombineAllLinkScripts) {
                    addressArrays.Add(groupName, validAddresses);
                }
                else {
                    var combinedAddresses = new List<string>();
                    combinedAddresses.AddRange(addressArrays["DefaultLocalGroup"]);
                    combinedAddresses.AddRange(validAddresses);
                    addressArrays["DefaultLocalGroup"] = combinedAddresses.ToArray();
                }
            }

            //remove this directory first
            var linkScriptsDirectory = $"{Application.dataPath}/Scripts/AutoAddressableLinks";
            if (Directory.Exists(linkScriptsDirectory)) {
                Directory.Delete(linkScriptsDirectory, true);
            }

            Directory.CreateDirectory(linkScriptsDirectory);

            if (m_CombineAllLinkScripts) {
                GenerateLinkCSharpPerGroup("CombinedResource", addressArrays["DefaultLocalGroup"]);
            }
            else {
                foreach (var addressArray in addressArrays) {
                    GenerateLinkCSharpPerGroup(addressArray.Key, addressArray.Value);
                }
            }
        }

        private string[] MarkAddressablePerGroup(string path, string addressableGroupName, SearchOption searchOption) {
            var result = new List<string>();
            var files = Directory.GetFiles(path, "*.*", searchOption);
            foreach (var file in files) {
                if (File.Exists(file)) {
                    var success = AutoAddressableHelper.MarkAddressable(addressableGroupName, file, out var address);
                    if (success) {
                        result.Add(address);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return result.ToArray();
        }

        private void GenerateLinkCSharpPerGroup(string groupName, string[] addresses) {
            string scriptTemplate = File.ReadAllText(TemplatePath);
            var scriptName = RemoveSpecialCharacters(groupName) + "Addresses";
            scriptTemplate = scriptTemplate.Replace("#GENERATETIME#", DateTime.UtcNow.ToUniversalTime().ToString(CultureInfo.CurrentCulture));
            scriptTemplate = scriptTemplate.Replace("#SCRIPTNAME#", scriptName);
            scriptTemplate = scriptTemplate.Replace("#ADDRESSCONTENT#", GetContentString(addresses));

            File.WriteAllText($"{Application.dataPath}/Scripts/AutoAddressableLinks/{scriptName}.cs", scriptTemplate);
            AssetDatabase.Refresh();
        }

        private string GetContentString(string[] addresses) {
            var result = "";
            foreach (var address in addresses) {
                var addressName = RemoveSpecialCharacters(address);
                result += $"\tpublic const string {addressName} = \"{address}\";\n";
            }

            return result;
        }

        string RemoveSpecialCharacters(string input) {
            return Regex.Replace(input, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}

#endif