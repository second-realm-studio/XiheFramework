using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Entry;

namespace XiheFramework.Utility.AStar {
    public class AStarGrid : MonoBehaviour {
        private Dictionary<int, AStarNode> m_AStarNodes = new();

        private void Start() {
            var size = Game.Blackboard.GetData<float>("AStarNodeSize");
        }
    }
}