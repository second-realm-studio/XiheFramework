using System;
using UnityEngine;

namespace XiheFramework {
    public class GridBlock : MonoBehaviour {
        public AStarNode aStarNode;

        private BoxCollider m_Collider;

        private void Start() {
            if (aStarNode == null) {
                aStarNode = GetComponent<AStarNode>();
            }

            m_Collider = GetComponent<BoxCollider>();

            aStarNode.InitNode();
            Game.Grid.RegisterBlock(aStarNode.gridX, aStarNode.gridY, this);

            var size = Game.Blackboard.GetData<float>("AStar.NodeSize");
            m_Collider.size = new Vector3(size, 0.2f, size);
        }
    }
}