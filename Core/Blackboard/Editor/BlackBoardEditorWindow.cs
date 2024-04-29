using System.Linq;
using UnityEditor;
using UnityEngine;
using XiheFramework.Core.Utility.DataStructure;

namespace XiheFramework.Core.Blackboard.Editor {
    public class BlackBoardEditorWindow : EditorWindow {
        private int m_IterationIndex;
        private readonly string[] m_Options = { "Group", "Type" };
        private Vector2 m_ScrollPos;
        private int m_SortingMode;

        private TreeNode<bool> m_States;

        private void Update() {
            Repaint();
        }

        private void OnEnable() {
            //GameCore.Event.Subscribe("BlackBoardChanged", OnBlackBoardChanged);
        }


        // private void OnSelectionChange() {
        //     Repaint();
        // }

        private void OnGUI() {
            if (!Application.isPlaying) {
                GUILayout.Label("DATA STATUS : NOT IN PLAY MODE", EditorStyles.boldLabel);
                m_SortingMode = EditorGUILayout.Popup("Sorting Mode:", m_SortingMode, m_Options);
                return;
            }

            if (GameCore.Blackboard == null) {
                GUILayout.Label("DATA STATUS : MISSING XIHE FRAMEWORK", EditorStyles.boldLabel);
                return;
            }

            var root = GameCore.Blackboard.GetDataPaths();
            if (root == null) {
                GUILayout.Label("DATA STATUS : DATA NULL", EditorStyles.boldLabel);
                return;
            }

            GUILayout.Label("DATA STATUS : LIVE", EditorStyles.boldLabel);

            m_SortingMode = EditorGUILayout.Popup("Sorting Mode:", m_SortingMode, m_Options);

            GUILayout.Space(10f);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            foreach (var key in GameCore.Blackboard.GetDataPathArray()) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key, EditorStyles.boldLabel);
                if (GameCore.Blackboard.GetData(key) != null)
                    GUILayout.Label(GameCore.Blackboard.GetData(key).ToString());
                else
                    GUILayout.Label("NULL");

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10f);

            //m_States = CreateStates(m_States, root);
            if (m_States == null) OnBlackBoardChanged(null, null); //bad use

            //BuildFoldout(m_States, root);
            //TreeViewLayout(root, 0);

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Xihe/BlackBoard Debug Window")]
        private static void Init() {
            // Get existing open window or if none, make a new one:
            var window = (BlackBoardEditorWindow)GetWindow(typeof(BlackBoardEditorWindow));
            window.Show();
        }

        private void OnBlackBoardChanged(object sender, object e) {
            m_States = new TreeNode<bool>(true);
            var dataPaths = GameCore.Blackboard.GetDataPaths();
            m_States = CreateStates(m_States, dataPaths);
        }

        private void TreeViewLayout(TreeNode<string> root, int depth) {
            if (root == null) return;

            GUILayout.BeginHorizontal();
            var tab = "";
            for (var i = 0; i < depth; i++) tab += "    ";

            GUILayout.Label(tab + root.Value);
            if (root.Children.Count == 0) {
                GUILayout.Label(GameCore.Blackboard.GetData(root.Value).GetType().ToString());
                GUILayout.Label(GameCore.Blackboard.GetData(root.Value).ToString());
            }

            GUILayout.EndHorizontal();

            depth++;

            foreach (var child in root.Children) TreeViewLayout(child, depth);
        }

        private TreeNode<bool> CreateStates(TreeNode<bool> target, TreeNode<string> source) {
            if (source == null) return null;

            foreach (var child in source.Children) {
                var node = target.AddChild(false);
                return CreateStates(node, child);
            }

            return null;
        }

        private void BuildFoldout(TreeNode<bool> root, TreeNode<string> label) {
            GUILayout.BeginHorizontal();
            root.Value = EditorGUILayout.Foldout(root.Value, label.Value);
            GUILayout.Label(label.Value);
            GUILayout.EndHorizontal();
            if (root.Value)
                for (var i = 0; i < root.Children.Count(); i++)
                    BuildFoldout(root.Children[i], label.Children[i]);
        }
    }
}