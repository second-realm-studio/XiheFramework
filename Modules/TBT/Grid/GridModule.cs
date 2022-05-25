using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XiheFramework {
    public class GridModule : GameModule {
        private Dictionary<int, GridBlock> m_Blocks = new Dictionary<int, GridBlock>();

        private void Start() {
            Game.Event.Subscribe("OnRequestPathToBlock", OnRequestPathToBlock);
        }

        private void OnRequestPathToBlock(object sender, object e) {
            var ns = sender as Transform;
            if (ns == null) {
                return;
            }

            var ne = e as GridBlock;
            if (ne == null) {
                return;
            }

            var path = GetVector3Path(ns.position, ne, false);
            if (path == null) {
                return;
            }

            Game.Event.Invoke("OnReceivePathToBlock", sender, path);
        }

        public void RegisterBlock(int x, int y, GridBlock block) {
            var key = CantorPairUtility.CantorPair(x, y);
            if (!m_Blocks.ContainsKey(key)) {
                m_Blocks.Add(key, block);
            }
            else {
                m_Blocks[key] = block;
            }
        }

        public Vector3[] GetVector3Path(Vector3 position, GridBlock targetBlock, bool includeDiagonalPath) {
            return GetVector3Path(position, targetBlock.aStarNode.gridX, targetBlock.aStarNode.gridY, includeDiagonalPath);
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
            if (path == null) {
                //can not find path
                return null;
            }

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

        private int GetNearestBlock(Vector3 position) {
            var lowest = float.MaxValue;
            var lowestKey = 0;
            foreach (var key in m_Blocks.Keys) {
                var dst = Vector3.Distance(m_Blocks[key].transform.position, position);
                if (dst < lowest) {
                    lowest = dst;
                    lowestKey = key;
                }
            }

            return lowestKey;
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }
    }
}