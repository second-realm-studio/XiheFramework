using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using XiheFramework;

[RequireComponent(typeof(Blackboard))]
public class InitializeGameData : MonoBehaviour {
    public Blackboard blackboard;

    // Start is called before the first frame update
    void Start() {
        var iterator = blackboard.GetVariables();
        foreach (var variable in iterator) {
            Game.Blackboard.SetData(variable.name, variable.value, BlackBoardDataType.Runtime);
        }
    }
}