using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XiheFramework {
    public class GridModule : GameModule {
        private Dictionary<int, GridBlock> m_Blocks = new Dictionary<int, GridBlock>();

        public void RegisterBlock(int x, int y, GridBlock block) {
            var key = CantorPairUtility.CantorPair(x, y);
            if (!m_Blocks.ContainsKey(key)) {
                m_Blocks.Add(key, block);
            }
            else {
                m_Blocks[key] = block;
            }
        }

        public Vector3[] GetVector3Path(Vector3 position, int targetX, int targetY, bool includeDiagonalPath) {
            var blockId = GetNearestBlock(position);
            CantorPairUtility.ReverseCantorPair(blockId, out int x, out int y);
            //a star
            return GetVector3Path(x, y, targetX, targetY, includeDiagonalPath);
        }

        public Vector3[] GetVector3Path(int originX, int originY, int targetX, int targetY, bool includeDiagonalPath) {
            List<Vector3> result = new List<Vector3>();

            var path = GetNodePath(originX, originY, targetX, targetY, includeDiagonalPath);
            foreach (var node in path) {
                result.Add(node.worldPosition);
            }

            return result.ToArray();
        }

        public AStarNode[] GetNodePath(int originX, int originY, int targetX, int targetY, bool includeDiagonalPath) {
            var key1 = CantorPairUtility.CantorPair(originX, originY);
            var key2 = CantorPairUtility.CantorPair(targetX, targetY);
            if (!m_Blocks.ContainsKey(key1) || !m_Blocks.ContainsKey(key2)) {
                return null;
            }

            Dictionary<int, AStarNode> source = m_Blocks.Keys.ToDictionary(key => key, key => m_Blocks[key].aStarNode);

            var path = AStar.GetPath(source, m_Blocks[key1].aStarNode, m_Blocks[key2].aStarNode, false);
            return path;
        }

        public int GetNearestBlock(Vector3 position) {
            return 0;
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }
    }
}