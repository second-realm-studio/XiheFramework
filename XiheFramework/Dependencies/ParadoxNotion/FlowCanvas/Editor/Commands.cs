#if UNITY_EDITOR

using UnityEditor;
using ParadoxNotion.Design;
using NodeCanvas.Editor;
using NodeCanvas.Framework;

namespace FlowCanvas.Editor
{

    public static class Commands
    {

        [MenuItem("Tools/ParadoxNotion/More Tools...", false, 1000)]
        public static void VisitHome() {
            Help.BrowseURL("http://paradoxnotion.com");
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Create/Global Scene Blackboard", false, 11)]
        public static void CreateGlobalSceneBlackboard() {
            Selection.activeObject = GlobalBlackboard.Create();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Preferred Types Editor")]
        public static void ShowPrefTypes() {
            TypePrefsEditorWindow.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Graph Console")]
        public static void OpenConsole() {
            GraphConsole.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Graph Explorer")]
        public static void OpenExplorer() {
            GraphExplorer.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Graph Refactor")]
        public static void OpenRefactor() {
            GraphRefactor.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Active Owners Overview")]
        public static void OpenOwnersOverview() {
            ActiveOwnersOverview.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/External Inspector Panel")]
        public static void ShowExternalInspector() {
            ExternalInspectorWindow.ShowWindow();
        }


        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Welcome Window")]
        public static void ShowWelcome() {
            WelcomeWindow.ShowWindow(typeof(FlowScript));
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Visit Website")]
        public static void VisitWebsite() {
            Help.BrowseURL("http://flowcanvas.paradoxnotion.com");
        }
    }
}

#endif