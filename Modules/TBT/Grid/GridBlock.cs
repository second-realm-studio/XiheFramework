using System;
using UnityEngine;

namespace XiheFramework {
    public class GridBlock : MonoBehaviour {
        public AStarNode aStarNode;

        private void Start() {
            if (aStarNode == null) {
                aStarNode = GetComponent<AStarNode>();
            }

            aStarNode.InitNode();
            Game.Grid.RegisterBlock(aStarNode.gridX, aStarNode.gridY, this);
        }
    }
}