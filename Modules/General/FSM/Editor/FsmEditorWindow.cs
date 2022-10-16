using System.Linq;
using UnityEditor;
using UnityEngine;
using XiheFramework;

public class FsmEditorWindow : EditorWindow {
    //string[] m_Options = {"Group", "Type"};
    //private int m_SortingMode;
    private Vector2 m_ScrollPos;

    private TreeNode<bool> m_States;
    private int m_IterationIndex;

    [MenuItem("Xihe/FSM Debug Window")]
    static void Init() {
        // Get existing open window or if none, make a new one:
        FsmEditorWindow window = (FsmEditorWindow) EditorWindow.GetWindow(typeof(FsmEditorWindow));
        window.Show();
    }

    private void OnEnable() {
        //Game.Event.Subscribe("BlackBoardChanged", OnBlackBoardChanged);
    }

    private void Update() {
        Repaint();
    }


    // private void OnSelectionChange() {
    //     Repaint();
    // }

    void OnGUI() {
        if (!Application.isPlaying) {
            GUILayout.Label("DATA STATUS : NOT IN PLAY MODE", EditorStyles.boldLabel);
            //m_SortingMode = EditorGUILayout.Popup("Sorting Mode:", m_SortingMode, m_Options);
            return;
        }

        var data = Game.Fsm.GetData();
        //var root = Game.Blackboard.GetDataPaths();
        if (data == null) {
            GUILayout.Label("DATA STATUS : DATA NULL", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label("DATA STATUS : LIVE", EditorStyles.boldLabel);

        //m_SortingMode = EditorGUILayout.Popup("Sorting Mode:", m_SortingMode, m_Options);

        GUILayout.Space(10f);

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

        GUILayout.BeginHorizontal();
        GUILayout.Label("State Machine", EditorStyles.boldLabel);
        GUILayout.Label("Current State", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
        
        foreach (var key in data.Keys) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(key, EditorStyles.boldLabel);
            GUILayout.Label(data[key].GetCurrentState());
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10f);

        EditorGUILayout.EndScrollView();
    }
}