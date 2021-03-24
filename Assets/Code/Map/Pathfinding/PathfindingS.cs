using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfindingS {
    // The A* Algorithm explained http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html
    // Explanation with animations and interactive graphs https://www.redblobgames.com/pathfinding/a-star/introduction.html
    // Visualization of different pathfinding algorithms https://movingai.com/SAS/SUB/
    public static List<PNodeS> FindPath_AStar(PathDataLayer pathingData, Vector2Int startPos, Vector2Int endPos, bool correctPositions = false) {
        #region SanityChecks
        // TODO: correct positions to move to the closest pathable position

        // Check for outside of bounds positions
        if (startPos.x < 0 || startPos.x >= pathingData.MapSize.x || startPos.y < 0 || startPos.y >= pathingData.MapSize.y) {
            Debug.LogError("Start Position is [out of bounds]");
            return null;
        } // Start Pos
        if (endPos.x < 0 || endPos.x >= pathingData.MapSize.x || endPos.y < 0 || endPos.y >= pathingData.MapSize.y) {
            Debug.LogError("End Position is [out of bounds]");
            return null;
        } // End Pos

        // If either the startPos or the endPos are non pathable, path won't be found
        if (pathingData.IsWalkable[startPos.x, startPos.y] == false || pathingData.IsWalkable[endPos.x, endPos.y] == false) {
            Debug.Log("Start/End Position is [unpathable]");
            return null;
        }
        #endregion

        PNodeS[,] nodes = new PNodeS[pathingData.MapSize.x, pathingData.MapSize.y];
        // Initialize nodes[]
        for (int x = 0; x < pathingData.MapSize.x; x++) {
            for (int y = 0; y < pathingData.MapSize.y; y++) {
                // Create nodes
                nodes[x, y] = new PNodeS(
                    new Vector2Int(x, y),
                    pathingData.IsWalkable[x, y],
                    pathingData.PathCost[x, y], 0, 0);
            }
        }

        PNodeS startNode = nodes[startPos.x, startPos.y];
        PNodeS endNode = nodes[endPos.x, endPos.y];

        //? Open & Closed sets
        List<PNodeS> OpenSet = new List<PNodeS> { startNode }; //? Add the start node to the open set
        List<PNodeS> ClosedSet = new List<PNodeS>();

        // Loop

        Debug.Log("Unable to find a path");
        return null; // In case a path could not be found
    }

    // List<PNodeS> TracebackPath(PNodeS endNode, PNodeS startNode)

    // int GetDistance(PNodeS a, PNodeS b)
}
