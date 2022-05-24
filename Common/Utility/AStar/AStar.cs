using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XiheFramework {
    public static class AStar {
        private static List<AStarNode> GetNeighbours(Dictionary<int, AStarNode> source, AStarNode node, bool includeDiagonal) {
            List<AStarNode> neighbours = new List<AStarNode>();

            if (includeDiagonal) {
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        if (x == 0 && y == 0)
                            continue;

                        int checkX = node.gridX + x;
                        int checkY = node.gridY + y;

                        var key = CantorPairUtility.CantorPair(checkX, checkY);
                        if (source.ContainsKey(key)) {
                            neighbours.Add(source[key]);
                        }
                    }
                }
            }
            else {
                var keys = new List<int>();
                keys.Add(CantorPairUtility.CantorPair(node.gridX - 1, node.gridY));
                keys.Add(CantorPairUtility.CantorPair(node.gridX + 1, node.gridY));
                keys.Add(CantorPairUtility.CantorPair(node.gridX, node.gridY + 1));
                keys.Add(CantorPairUtility.CantorPair(node.gridX, node.gridY - 1));

                neighbours.AddRange(from key in keys where source.ContainsKey(key) select source[key]);
            }

            return neighbours;
        }

        public static AStarNode[] GetPath(Dictionary<int, AStarNode> source, AStarNode startNode, AStarNode targetNode, bool includeDiagonalPath) {
            List<AStarNode> openSet = new List<AStarNode>();
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                AStarNode node = openSet[0];
                for (int i = 1; i < openSet.Count; i++) {
                    if (openSet[i].TotalDst < node.TotalDst || openSet[i].TotalDst == node.TotalDst) {
                        if (openSet[i].toDst < node.toDst)
                            node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                if (node == targetNode) {
                    var path = RetracePath(startNode, targetNode);
                    return path;
                }

                foreach (AStarNode neighbour in GetNeighbours(source, node, includeDiagonalPath)) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                        continue;
                    }

                    int newCostToNeighbour = node.fromDst + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.fromDst || !openSet.Contains(neighbour)) {
                        neighbour.fromDst = newCostToNeighbour;
                        neighbour.toDst = GetDistance(neighbour, targetNode);
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            //cant find path
            return null;
        }

        private static AStarNode[] RetracePath(AStarNode startNode, AStarNode endNode) {
            List<AStarNode> path = new List<AStarNode>();
            AStarNode currentNode = endNode;

            while (currentNode != startNode) {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            return path.ToArray();
        }

        private static int GetDistance(AStarNode nodeA, AStarNode nodeB) {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}