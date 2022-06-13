using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public static class GridSvc {
    public static Vector3 GetGridPosition(int x, int y) {
        return Game.Grid.GetGridPosition(x, y);
    }

    public static void GetGridPosition(Vector3 position, out int x, out int y) {
        Game.Grid.GetGridCoordinate(position, out int gx, out int gy);
        x = gx;
        y = gy;
    }

    public static void GetBlockCoordinate(Vector3 position, out int x, out int y) {
        var block = Game.Grid.GetNearestBlock(position);
        x = block.aStarNode.gridX;
        y = block.aStarNode.gridY;
    }
}