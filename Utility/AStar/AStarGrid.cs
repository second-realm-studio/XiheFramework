using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core;

namespace XiheFramework.Utility.AStar {
    public class AStarGrid : MonoBehaviour {
        private Dictionary<int, AStarNode> m_AStarNodes = new();

        private void Start() {
            var size = GameCore.Blackboard.GetData<float>("AStarNodeSize");
        }
    }
}