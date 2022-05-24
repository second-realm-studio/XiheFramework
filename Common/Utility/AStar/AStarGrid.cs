using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class AStarGrid : MonoBehaviour {
        private Dictionary<int, AStarNode> m_AStarNodes=new Dictionary<int, AStarNode>();
    
        private void Start() {
            var size=Game.Blackboard.GetData<float>("AStarNodeSize");
        }
    }
}

