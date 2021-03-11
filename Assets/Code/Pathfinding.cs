using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding {
    // The A* Algorithm explained http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html
    // Explanation with animations and interactive graphs https://www.redblobgames.com/pathfinding/a-star/introduction.html
    // Visualization of different pathfinding algorithms https://movingai.com/SAS/SUB/
    public static List<PathNode> FindPath_AStar(PathDataLayer pathingData, Vector2Int startPos, Vector2Int endPos) {
        Debug.Log("Let the party start!");

        // If either the startPos or the endPos are non pathable, path won't be found
        if (pathingData.IsWalkable[startPos.x, startPos.y] == false || pathingData.IsWalkable[endPos.x, endPos.y] == false) {
            Debug.Log("No path: Invalid Position");
            return null;
        }
        Debug.Log("Positions are valid");

        //? Create a node for the start and end positions
        PathNode startNode = new PathNode(pathingData, startPos);
        PathNode endNode   = new PathNode(pathingData, endPos);
        Debug.Log("Nodes created");

        //? Open & Closed sets
        List<PathNode> OpenSet = new List<PathNode> { startNode }; //? Add the start node to the open set
        List<PathNode> ClosedSet = new List<PathNode>();

        // Loop though all the nodes until OpenSet => empty, or currentNode => endPos
        while (OpenSet.Count > 0) {
            PathNode currentNode = OpenSet[0];

            // Find the node in the OpenSet with the lowest fCost
            for (int i = 1; i < OpenSet.Count; i++) {
                if (OpenSet[i].fCost <= currentNode.fCost || OpenSet[i].hCost < currentNode.hCost) {
                    currentNode = OpenSet[i];
                }
            }

            if (currentNode == endNode) {
                Debug.Log("Path found");
                return TracebackPath(startNode, endNode);
            }

            // Move the current node from the OpenSet => ClosedSet
            OpenSet.Remove(currentNode);
            ClosedSet.Add(currentNode);


            // Cicle through all the neighbours
            foreach (PathNode neighbour in currentNode.GetNeighbours(pathingData)) {
                // If a neighbour is Un-Walkable / Un-Pathable or its on the ClosedSet => skip
                if (!neighbour.isWalkable) continue;
                if (ClosedSet.Contains(neighbour)) continue;

                // Calculate new gCost to neighbour
                float newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                newCostToNeighbour += currentNode.pathCost; //? An attempt to implement tile's path cost
                if (newCostToNeighbour < neighbour.gCost) {
                    neighbour.cameFrom = currentNode;
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(currentNode, endNode);

                    if (!OpenSet.Contains(neighbour)) OpenSet.Add(neighbour);
                }
            }
        }
        Debug.Log("No path");
        return null; // In case a path could not be found
    }
    private static List<PathNode> TracebackPath(PathNode startNode, PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.cameFrom;
        }
        // Reverse the path, as currently it starts at the end
        path.Reverse();
        return path;
    }

    private static float GetDistance(PathNode nodeA, PathNode nodeB, float D = 7, float D2 = 5) {
        float dX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        float dY = Mathf.Abs(nodeA.position.y - nodeB.position.y);

        // Manhatan Diagonal distance
        //return D * Mathf.Max(dX, dY) + (D2 - D) * Mathf.Min(dX, dY);
        //return D * (dX + dY) + (D2 - 2 * D) * Mathf.Min(dX, dY);

        if (dX > dY) return (D * (dX - dY) + D2 * dY);
                     return (D * (dY - dX) + D2 * dX);
    }
}