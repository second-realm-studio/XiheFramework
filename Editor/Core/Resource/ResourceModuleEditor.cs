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
        private const string TemplatePath = "Assets/XiheFramework/Editor/Core/Resource/AddressWrapperTemplate.txt";
        private const string LinkScriptsDirectory = "Assets/Scripts/GameConstant/";

        private bool m_CombineAllLinkScripts = true;
        private bool m_ReadyGenerate;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUILayout.Space(10f);

            GUILayout.Label("Addressable", EditorStyles.boldLabel);
            if (Application.isPlaying) {
                EditorGUILayout.HelpBox("Auto Addressable is disabled during Play Mode", MessageType.Warning);
                return;
            }

            m_ReadyGenerate = true;

            if (!AssetDatabase.IsValidFolder(AddressableResourcesRoot)) {
                m_ReadyGenerate = false;
                if (GUILayout.Button("Create Addressable Resources Folder")) {
                    AssetDatabase.CreateFolder("Assets", "AddressableResources");
                }
            }
            else {
                GUI.enabled = true;
                GUILayout.Label("Resource Folder: ");
                GUI.enabled = false;
                GUILayout.TextField(AddressableResourcesRoot);
                GUILayout.Space(2f);
                GUI.enabled = true;
            }

            // m_CombineAllLinkScripts = GUILayout.Toggle(m_CombineAllLinkScripts, "Combine Link Scripts");

            if (!File.Exists(TemplatePath)) {
                EditorGUILayout.HelpBox("Template File not found! Do not move XiheFramework folder!", MessageType.Error);
                m_ReadyGenerate = false;
            }

            if (!m_ReadyGenerate) {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Generate Sprite Assets")) {
                GenerateSpriteAssetsOnly();
            }

            if (GUILayout.Button("Generate Address Wrapper")) {
                GenerateAddressWrapper();
            }

            GUI.enabled = true;
        }

        private void GenerateSpriteAssetsOnly() {
            var files = Directory.GetFiles(AddressableResourcesRoot, "*.*", SearchOption.AllDirectories);
            int count = 0;

            foreach (var file in files) {
                if (!File.Exists(file)) continue;

                if (AssetDatabase.GetMainAssetTypeAtPath(file) == typeof(Texture2D)) {
                    bool success = TryCreateSpriteAssetFromTexture(file, out _);
                    if (success) count++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"生成 Sprite Asset 完成，共生成：{count} 个");
        }

        private void GenerateAddressWrapper() {
#if USE_ADDRESSABLE
            m_CombineAllLinkScripts = true;
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) {
                Debug.LogWarning("Addressable Asset Settings not found! Go to: Window->Asset Management->Addressables->Groups, Create Addressable Asset Settings.");
                return;
            }

            var addressInfoArrays = new Dictionary<string, AssetNameAddressPair[]>();
            MarkAssetsAddressable(AddressableResourcesRoot, "Default Local Group", SearchOption.TopDirectoryOnly,
                out var validAddressInfos);
            addressInfoArrays.Add("DefaultLocalGroup", validAddressInfos);

            var subFolders = Directory.GetDirectories(AddressableResourcesRoot, "*", SearchOption.TopDirectoryOnly);
            foreach (var path in subFolders) {
                var groupName = new DirectoryInfo(path).Name;
                MarkAssetsAddressable(path, groupName, SearchOption.AllDirectories, out validAddressInfos);
                if (!m_CombineAllLinkScripts) {
                    addressInfoArrays.Add(groupName, validAddressInfos);
                }
                else {
                    var combinedAddresses = new List<AssetNameAddressPair>();
                    combinedAddresses.AddRange(addressInfoArrays["DefaultLocalGroup"]);
                    combinedAddresses.AddRange(validAddressInfos);
                    addressInfoArrays["DefaultLocalGroup"] = combinedAddresses.ToArray();
                }
            }

            //detect if there are empty addressable groups
            foreach (var group in settings.groups) {
                if (group.entries.Count == 0 && !group.IsDefaultGroup()) {
                    Debug.LogWarning($"AddressableGroup {group.Name} is empty, consider deleting it.");
                }
            }

            if (!Directory.Exists(LinkScriptsDirectory)) {
                Directory.CreateDirectory(LinkScriptsDirectory);
            }

            var defaultGroupEmpty = addressInfoArrays.ContainsKey("DefaultLocalGroup") && addressInfoArrays["DefaultLocalGroup"].Length == 0;
            if (addressInfoArrays.Count == 0 || defaultGroupEmpty) {
                Debug.LogWarning("No addressable assets found! Empty ResourceAddresses script generated.");
                WriteWrapperScript("Resource", null, false);
            }

            if (m_CombineAllLinkScripts) {
                WriteWrapperScript("Resource", addressInfoArrays["DefaultLocalGroup"]);
            }
            else {
                foreach (var entry in addressInfoArrays) {
                    WriteWrapperScript(entry.Key, entry.Value);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Generated Address Wrapper Script at " + LinkScriptsDirectory);
#else
            Debug.LogError("Please import Addressable Package and define USE_ADDRESSABLE in your project settings: Player->Other Settings->Scripting Define Symbols");
#endif
        }
#if USE_ADDRESSABLE
        private void MarkAssetsAddressable(string path, string addressableGroupName, SearchOption searchOption, out AssetNameAddressPair[] addressInfos) {
            var addressesResult = new List<AssetNameAddressPair>();
            var files = Directory.GetFiles(path, "*.*", searchOption);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(addressableGroupName);
            if (group == null) {
                group = settings.CreateGroup(addressableGroupName, false, false, true, null);
                group.AddSchema<BundledAssetGroupSchema>();
                group.AddSchema<ContentUpdateGroupSchema>();
                Debug.Log("Created Addressable Group: " + addressableGroupName);
            }

            foreach (var file in files) {
                if (!File.Exists(file)) continue;

                // 始终尝试标记原始贴图
                var successTex = MarkAssetAddressable(addressableGroupName, file, out var addressInfoTex);
                if (successTex) {
                    addressesResult.Add(addressInfoTex);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            addressInfos = addressesResult.ToArray();
        }

        private bool TryCreateSpriteAssetFromTexture(string texturePath, out string spriteAssetPath) {
            spriteAssetPath = null;

            // 载入贴图
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null) {
                Debug.LogWarning($"无法加载贴图: {texturePath}");
                return false;
            }

            if (texture.name != "Hole_Jackpot") {
                return false;
            }

            // 获取 Importer 设置
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer == null || importer.textureType != TextureImporterType.Sprite) {
                Debug.LogWarning($"贴图不是 Sprite 类型: {texturePath}");
                return false;
            }

            // 只支持单图（多 Sprite 要走 spriteImportData[]）
            if (importer.spriteImportMode != SpriteImportMode.Single) {
                Debug.LogWarning($"暂不支持多图切片 Sprite: {texturePath}");
                return false;
            }

            // 获取 pivot、pixelsPerUnit、border
            Vector2 pivot = importer.spritePivot;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            importer.GetSourceTextureWidthAndHeight(out int originalWidth, out int originalHeight);
            float scaleX = texture.width / (float)originalWidth;
            float scaleY = texture.height / (float)originalHeight;
            Vector4 originalBorder = importer.spriteBorder;
            Vector4 scaledBorder = new Vector4(
                originalBorder.x * scaleX,
                originalBorder.y * scaleY,
                originalBorder.z * scaleX,
                originalBorder.w * scaleY
            );

            float ppu = importer.spritePixelsPerUnit / scaleX;

            // 创建 Sprite
            Sprite sprite = Sprite.Create(texture, rect, pivot, ppu, 1, SpriteMeshType.FullRect, scaledBorder, false);
            sprite.name = Path.GetFileNameWithoutExtension(texturePath) + "_Sprite";

            // 保存成 .asset 文件
            spriteAssetPath = Path.Combine(Path.GetDirectoryName(texturePath),
                Path.GetFileNameWithoutExtension(texturePath) + "_Sprite.asset");

            if (File.Exists(spriteAssetPath)) {
                AssetDatabase.DeleteAsset(spriteAssetPath);
            }

            AssetDatabase.CreateAsset(sprite, spriteAssetPath);
            AssetDatabase.ImportAsset(spriteAssetPath);
            return true;
        }


        private static bool MarkAssetAddressable(string folderName, string assetPath, out AssetNameAddressPair addressInfo) {
            addressInfo = new AssetNameAddressPair();
            if (string.IsNullOrEmpty(folderName)) {
                return false;
            }

            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) {
                return false;
            }

            var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (asset == null) {
                settings.RemoveAssetEntry(guid);
                return false;
            }

            string assetAddress = "";

            switch (asset) {
                case GameObject gameObject:
                    var gameEntity = gameObject.GetComponent<GameEntity>();
                    if (gameEntity != null) {
                        assetAddress = $"{gameEntity.GroupName}_{asset.name}";
                    }
                    else {
                        assetAddress = $"{type.Name}_{asset.name}";
                    }

                    break;
                default:
                    assetAddress = $"{type.Name}_{asset.name}";
                    break;
            }

            var group = settings.FindGroup(folderName);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = assetAddress;
            addressInfo.assetName = $"{folderName}_{asset.name}";
            addressInfo.address = assetAddress;
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            return true;
        }

        private void WriteWrapperScript(string groupName, AssetNameAddressPair[] addressInfo, bool ignoreEmpty = true) {
            string scriptTemplate = File.ReadAllText(TemplatePath);
            var scriptName = RemoveSpecialCharacters(groupName) + "Addresses";
            scriptTemplate = scriptTemplate.Replace("#GENERATETIME#", DateTime.UtcNow.ToUniversalTime().ToString(CultureInfo.CurrentCulture));
            scriptTemplate = scriptTemplate.Replace("#SCRIPTNAME#", scriptName);
            string content = string.Empty;
            if (addressInfo != null && addressInfo.Length > 0) {
                content = GetContentString(addressInfo);
            }

            if (ignoreEmpty && string.IsNullOrEmpty(content)) {
                return;
            }

            scriptTemplate = scriptTemplate.Replace("#ADDRESSCONTENT#", content);

            File.WriteAllText($"{LinkScriptsDirectory}{scriptName}.cs", scriptTemplate);
        }

        private string GetContentString(AssetNameAddressPair[] addressInfo) {
            var result = "";
            for (var i = 0; i < addressInfo.Length; i++) {
                var assetName = addressInfo[i].assetName;
                var address = addressInfo[i].address;
                assetName = RemoveSpecialCharacters(assetName);
                result += $"\t\tpublic const string {assetName} = \"{address}\";\n";
            }

            if (string.IsNullOrEmpty(result)) {
                return null;
            }

            result = result[..^1];

            return result;
        }

        private string RemoveSpecialCharacters(string input) {
            //if first char is a number
            string output = Regex.Replace(input, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
            if (char.IsDigit(output[0])) {
                output = "_" + output;
            }

            return output;
        }


        public struct AssetNameAddressPair {
            public string assetName;
            public string address;
        }
#endif
    }
}