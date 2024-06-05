using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using System.IO;

namespace XiheFramework.Editor.Utility.XiheWizard {
    public class XiheFrameworkWizard : UnityEditor.EditorWindow {
        private bool useAddressable = false;
        private bool useCinemachine = false;
        private bool useRewired = false;
        private bool useTMP = false;
        private bool useWwise = false;

        private string asmdefPath = "Assets/XiheFramework/XiheFramework.asmdef";

        [MenuItem("XiheFramework/Setup Wizard", priority = -1)]
        public static void ShowWindow() {
            GetWindow<XiheFrameworkWizard>("XiheFramework Wizard");
        }

        private void OnEnable() {
            BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup currentBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(currentBuildTarget);
            RefreshSymbols(currentBuildTargetGroup);
        }

        private void OnGUI() {
            GUILayout.Label("XiheFramework Setup Wizard", new GUIStyle(GUI.skin.label) { fontSize = 20, fixedHeight = 40 });
            GUILayout.Space(5);
            GUILayout.Label("Current Build Target:", EditorStyles.boldLabel);
            GUILayout.Label(EditorUserBuildSettings.activeBuildTarget.ToString(), new GUIStyle(GUI.skin.label) { fontSize = 15, fixedHeight = 30 });

            GUILayout.Space(10);
            GUILayout.Label("ASMDEF File Path:", EditorStyles.boldLabel);
            asmdefPath = GUILayout.TextField(asmdefPath);
            GUILayout.Space(10);

            GUILayout.Label("Enable AFTER Importing Plugins:", EditorStyles.boldLabel);

            // Display checkboxes and check for changes
            bool anyChange = false;

            bool newUseAddressable = GUILayout.Toggle(useAddressable, "USE_ADDRESSABLE");
            if (newUseAddressable != useAddressable) {
                useAddressable = newUseAddressable;
                anyChange = true;
            }

            bool newUseCinemachine = GUILayout.Toggle(useCinemachine, "USE_CINEMACHINE");
            if (newUseCinemachine != useCinemachine) {
                useCinemachine = newUseCinemachine;
                anyChange = true;
            }

            bool newUseRewired = GUILayout.Toggle(useRewired, "USE_REWIRED");
            if (newUseRewired != useRewired) {
                useRewired = newUseRewired;
                anyChange = true;
            }

            bool newUseTMP = GUILayout.Toggle(useTMP, "USE_TMP");
            if (newUseTMP != useTMP) {
                useTMP = newUseTMP;
                anyChange = true;
            }

            bool newUseWwise = GUILayout.Toggle(useWwise, "USE_WWISE");
            if (newUseWwise != useWwise) {
                useWwise = newUseWwise;
                anyChange = true;
            }

            if (anyChange) {
                ApplySymbols();
            }
        }

        private void ApplySymbols() {
            BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup currentBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(currentBuildTarget);

            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentBuildTargetGroup);
            List<string> newSymbols = currentSymbols.Split(';').ToList();

            // Update the symbols managed by XiheFramework
            UpdateSymbol(newSymbols, useAddressable, "USE_ADDRESSABLE");
            UpdateSymbol(newSymbols, useCinemachine, "USE_CINEMACHINE");
            UpdateSymbol(newSymbols, useRewired, "USE_REWIRED");
            UpdateSymbol(newSymbols, useTMP, "USE_TMP");
            UpdateSymbol(newSymbols, useWwise, "USE_WWISE");

            var symbolString = string.Join(";", newSymbols.ToArray());

            // Set new symbols
            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentBuildTargetGroup, symbolString);
        }

        private void UpdateSymbol(List<string> symbols, bool isEnabled, string symbol) {
            if (isEnabled && !symbols.Contains(symbol)) {
                symbols.Add(symbol);
            } else if (!isEnabled && symbols.Contains(symbol)) {
                symbols.Remove(symbol);
            }
        }

        private void RefreshSymbols(BuildTargetGroup buildTargetGroup) {
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            List<string> splitSymbols = currentSymbols.Split(';').ToList();

            useAddressable = splitSymbols.Contains("USE_ADDRESSABLE");
            useCinemachine = splitSymbols.Contains("USE_CINEMACHINE");
            useRewired = splitSymbols.Contains("USE_REWIRED");
            useTMP = splitSymbols.Contains("USE_TMP");
            useWwise = splitSymbols.Contains("USE_WWISE");
        }
    }
}
