using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XiheFramework {
    public class GridModule : GameModule {
        class WalkablePair {
            public Vector3 position;
            public int cantorPair;
            public bool walkable;


            public WalkablePair(Vector3 position, int cantorPair, bool walkable) {
                this.position = position;
                this.cantorPair = cantorPair;
                this.walkable = walkable;
            }
        }

        private readonly Dictionary<int, GridBlock> m_Blocks = new Dictionary<int, GridBlock>();

        private bool m_FirstFrameProcessed = false;
        private Queue<WalkablePair> m_SetWalkableQueue = new Queue<WalkablePair>();

        private void Start() {
            Game.Event.Subscribe("OnRequestPathToBlock", OnRequestPathToBlock);
        }

        private void OnRequestPathToBlock(object sender, object e) {
            var ne = e as RequestPathArgs;
            if (ne == null) {
                return;
            }

            // //start block is equal to target block
            // if (GetNearestBlock(ne.startPos) == ne.targetBlock) {
            //     return;
            // }

            var target = ne.targetBlock;

            while (!target.aStarNode.walkable) {
                //if start pos'nearest block is the target block, pass it
                //this happens when the object is standing on the new target block
                if (target == GetNearestBlock(ne.startPos)) {
                    return;
                }

                //get closest block
                var size = Game.Blackboard.GetData<float>("AStar.NodeSize");
                var blockPos = target.transform.position;
                var newBlockPos = blockPos + (ne.startPos - blockPos).normalized * size;

                target = GetNearestBlock(newBlockPos);
            }

            var path = GetVector3Path(ne.startPos, ne.targetBlock, ne.includeDiagonalPath);
            if (path == null) {
                return;
            }

            //                                                  object, vector3[]
            Game.Event.Invoke("OnReceivePathToBlock", sender, path);
        }

        public void SetWalkable(int x, int y, bool isWalkable) {
            var key = CantorPairUtility.CantorPair(x, y);
            AddWalkableQueue(key, isWalkable);
        }

        public void SetWalkable(Vector3 position, bool isWalkable) {
            // var key = GetNearestBlockId(position);
            AddWalkableQueue(position, isWalkable);
        }

        void AddWalkableQueue(int id, bool isWalkable) {
            // if (m_Blocks.ContainsKey(id)) {
            //     m_Blocks[id].aStarNode.walkable = isWalkable;
            // }

            m_SetWalkableQueue.Enqueue(new WalkablePair(Vector3.zero, id, isWalkable));
        }

        void AddWalkableQueue(Vector3 position, bool isWalkable) {
            // if (m_Blocks.ContainsKey(id)) {
            //     m_Blocks[id].aStarNode.walkable = isWalkable;
            // }

            m_SetWalkableQueue.Enqueue(new WalkablePair(position, -1, isWalkable));
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
            var blockId = GetNearestBlockId(position);
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

            var path = AStar.GetPath(source, m_Blocks[key1].aStarNode, m_Blocks[key2].aStarNode, includeDiagonalPath);
            return path;
        }

        public GridBlock GetNearestBlock(Vector3 position) {
            return m_Blocks[GetNearestBlockId(position)];
        }

        private int GetNearestBlockId(Vector3 position) {
            var lowest = float.MaxValue;
            var lowestKey = 0;
            foreach (var key in m_Blocks.Keys) {
                if (m_Blocks[key] != null) {
                    var dst = Vector3.Distance(m_Blocks[key].transform.position, position);
                    if (dst < lowest) {
                        lowest = dst;
                        lowestKey = key;
                    }
                }
            }

            return lowestKey;
        }

        public Vector3 GetGridPosition(int x, int y) {
            var key = CantorPairUtility.CantorPair(x, y);
            if (m_Blocks.ContainsKey(key)) {
                return m_Blocks[key].aStarNode.worldPosition;
            }

            return Vector3.zero;
        }


        public void GetGridCoordinate(Vector3 position, out int x, out int y) {
            var block = GetNearestBlock(position);

            x = block.aStarNode.gridX;
            y = block.aStarNode.gridY;
        }

        public void RequestPath(object sender, Vector3 currentPosition, Vector3 targetPosition, bool includeDiagonalPath) {
            Game.Event.Invoke("OnRequestPathToBlock", sender,
                new RequestPathArgs(currentPosition, Game.Grid.GetNearestBlock(targetPosition), includeDiagonalPath));
        }

        public override void Update() {
            if (!m_FirstFrameProcessed) {
                m_FirstFrameProcessed = true;
                return;
            }

            while (m_SetWalkableQueue.Count > 0) {
                var item = m_SetWalkableQueue.Dequeue();
                int key = -1;
                if (item.cantorPair == -1) {
                    key = GetNearestBlockId(item.position);
                }
                else {
                    key = item.cantorPair;
                }

                if (m_Blocks.ContainsKey(key)) {
                    m_Blocks[key].aStarNode.walkable = item.walkable;
                }
            }
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }
    }
}