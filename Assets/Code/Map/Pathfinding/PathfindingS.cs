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
			Debug.LogWarning("Start/End Position is [unpathable]");
			return null;
		}
		#endregion
		PNodeS[,] nodes = new PNodeS[pathingData.MapSize.x, pathingData.MapSize.y];
		bool[,] walkableMap = pathingData.IsWalkable;
		int[,] pathCosts = pathingData.PathCost;

		// Initialize nodes[]
		for (int x = 0; x < pathingData.MapSize.x; x++) {
			for (int y = 0; y < pathingData.MapSize.y; y++) {
				// Create nodes
				nodes[x, y] = new PNodeS(
					new Vector2Int(x, y),
					int.MaxValue,
					int.MaxValue);
			}
		}

		PNodeS startNode = nodes[startPos.x, startPos.y];
		PNodeS endNode = nodes[endPos.x, endPos.y];

		// Open & Closed sets
		List<PNodeS> OpenSet = new List<PNodeS> { startNode }; //? Add the start node to the open set
		List<PNodeS> ClosedSet = new List<PNodeS>();

		// Loop
		while (OpenSet.Count > 0) {
			PNodeS currentNode = OpenSet[0];

			// If currentNode == endNode => Path Found
			if (currentNode == endNode) {
				Debug.Log("Path found");
				return TracebackPath(endNode, startNode);
			}

			// Find the node in the OpenSet with the lowest fCost
			for (int i = 1; i < OpenSet.Count; i++) {
				if (OpenSet[i].fCost < currentNode.fCost || OpenSet[i].fCost < currentNode.fCost && OpenSet[i].hCost < currentNode.hCost) {
					currentNode = OpenSet[i];
				}
			}

			// Move the current node from the OpenSet => ClosedSet
			ClosedSet.Add(currentNode);
			OpenSet.Remove(currentNode);

			// Cycle through all the neighbours
			foreach (PNodeS neighbour in GetNeighbours(currentNode.position, nodes, pathingData.MapSize, walkableMap)) {
				// If a neighbour is Un-Walkable / Un-Pathable or its on the ClosedSet => skip
				if (!walkableMap[neighbour.position.x, neighbour.position.y]) continue;
				if (ClosedSet.Contains(neighbour)) continue;

				// Calculate new gCost
				int newCost = currentNode.gCost + pathCosts[neighbour.position.x, neighbour.position.y];

				if (newCost < neighbour.gCost) {
					neighbour.breadcrumbs = currentNode;
					neighbour.gCost = newCost;
					neighbour.hCost = GetDistance(currentNode, endNode);

					if (!OpenSet.Contains(neighbour)) OpenSet.Add(neighbour);
				}
			}
		}

		Debug.Log("Unable to find a path");
		return null; // In case a path could not be found
	}

	private static List<PNodeS> TracebackPath(PNodeS endNode, PNodeS startNode) {
		List<PNodeS> path = new List<PNodeS>();
		PNodeS currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.breadcrumbs;
		}
		// Reverse the path, as currently it starts at the end
		path.Reverse();
		return path;
	}

	private static int GetDistance(PNodeS a, PNodeS b, int movementCost = 14, int diagonalMovementCost = 10) {
		int dX = Mathf.Abs(b.position.x - a.position.x);
		int dY = Mathf.Abs(b.position.y - a.position.y);

		if (dX > dY) return (movementCost * (dX - dY) + diagonalMovementCost * dY);
					 return (movementCost * (dY - dX) + diagonalMovementCost * dX);
	} // Manhattan distance

	private static List<PNodeS> GetNeighbours(Vector2Int nodePos, PNodeS[,] nodes, Vector2Int mapSize, bool[,] walkable) {
		//Debug.Log($"Node: [{nodePos.x},{nodePos.y}]");

		List<PNodeS> neighbours = new List<PNodeS>();
		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if (x == 0 && y == 0) continue; // Skip (0,0), it's this node

				// Check for [outside of bounds] positions, and skip those "neighbours"
				if ((nodePos.x + x) < 0 || (nodePos.x + x) >= mapSize.x) continue; // Check X axis
				if ((nodePos.y + y) < 0 || (nodePos.y + y) >= mapSize.y) continue; // Check Y axis

				// Skip non walkable nodes
				if (!walkable[nodePos.x + x, nodePos.y + y]) continue;

				// Add the Neighbour to the list
				//Debug.Log($"Neighbour: [{nodePos.x + x},{nodePos.y + y}]");
				neighbours.Add(nodes[nodePos.x + x, nodePos.y + y]);
			}
		}
		return neighbours;
	}
}
