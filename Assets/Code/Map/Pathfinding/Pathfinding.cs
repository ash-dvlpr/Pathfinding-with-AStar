using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding {
	// The A* Algorithm explained: http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html
	// Explanation with animations and interactive graphs: https://www.redblobgames.com/pathfinding/a-star/introduction.html
	// Visualization of different pathfinding algorithms: https://movingai.com/SAS/SUB/
	// Wikipedia article: https://en.wikipedia.org/wiki/A*_search_algorithm
	public static List<Vector2Int> FindPath_AStar(PathDataLayer pathingData, Vector2Int startPos, Vector2Int endPos) {
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
		PNode[,] nodes = new PNode[pathingData.MapSize.x, pathingData.MapSize.y];
		bool[,] pathableMap = pathingData.IsWalkable;
		int[,] pathCosts = pathingData.PathCost;

		// Initialize nodes[]
		for (int x = 0; x < pathingData.MapSize.x; x++) {
			for (int y = 0; y < pathingData.MapSize.y; y++) {
				nodes[x, y] = new PNode(new Vector2Int(x, y));
			}
		}

		PNode startNode = nodes[startPos.x, startPos.y]; 
		PNode endNode = nodes[endPos.x, endPos.y];
		startNode.gCost = 0; // Starting node gCost

		List<PNode> OpenSet = new List<PNode> { startNode }; //? Add the start node to the open set

		// Loop
		while (OpenSet.Count > 0) {
			PNode currentNode = OpenSet[0];

			// Find the node in the OpenSet with the lowest fCost
			for (int i = 1; i < OpenSet.Count; i++) {
				if (OpenSet[i].fCost < currentNode.fCost || OpenSet[i].fCost < currentNode.fCost && OpenSet[i].hCost < currentNode.hCost) {
					currentNode = OpenSet[i];
				}
			}

			// Move the current node from the OpenSet => ClosedSet
			currentNode.closed = true;
			OpenSet.Remove(currentNode);

			// Path Found <= If currentNode == endNode 
			if (currentNode == endNode) {
				//Debug.Log("Path found");
				return TracebackPath(currentNode, startNode);
			}

			// Cycle through all the neighbours
			foreach (PNode neighbour in GetNeighbours(currentNode.position, nodes, pathingData.MapSize, pathableMap)) {
				if (neighbour.closed) continue; // Already checked

				int totalGCost =
					  currentNode.gCost + GetDistance(currentNode, neighbour)
					+ pathCosts[neighbour.position.x, neighbour.position.y]; // Weights

				if (totalGCost < neighbour.gCost) {
					neighbour.breadcrumbs = currentNode;
					neighbour.gCost = totalGCost;
					neighbour.hCost = GetDistance(currentNode, endNode);

					if (!OpenSet.Contains(neighbour)) OpenSet.Add(neighbour);
				}
			}
		}
		//Debug.Log("Unable to find a path");
		return null; // In case a path could not be found
	}
	public static List<Vector2Int> FindPath_AStar_Heap(PathDataLayer pathingData, Vector2Int startPos, Vector2Int endPos) {
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
		PNode[,] nodes = new PNode[pathingData.MapSize.x, pathingData.MapSize.y];
		bool[,] pathableMap = pathingData.IsWalkable;
		int[,] pathCosts = pathingData.PathCost;

		// Initialize nodes[]
		for (int x = 0; x < pathingData.MapSize.x; x++) {
			for (int y = 0; y < pathingData.MapSize.y; y++) {
				nodes[x, y] = new PNode(new Vector2Int(x, y));
			}
		}

		PNode startNode = nodes[startPos.x, startPos.y]; 
		PNode endNode = nodes[endPos.x, endPos.y];
		startNode.gCost = 0; // Starting node gCost

		int MaxHeapSize = pathingData.MapSize.x * pathingData.MapSize.y;
		MinHeap<PNode> OpenSet = new MinHeap<PNode>(MaxHeapSize); //? Add the start node to the open set
		OpenSet.Add(startNode);

		// Loop
		while (OpenSet.Count > 0) {
			PNode currentNode = OpenSet.RemoveFirst(); // As this heap is sorted by fCost, no need to find the lowest fCost node
			currentNode.closed = true;

			// Path Found <= If currentNode == endNode 
			if (currentNode == endNode) {
				//Debug.Log("Path found");
				return TracebackPath(currentNode, startNode);
			}


			// Cycle through all the neighbours
			foreach (PNode neighbour in GetNeighbours(currentNode.position, nodes, pathingData.MapSize, pathableMap)) {
				if (neighbour.closed) continue; // Already checked

				int totalGCost =
					  currentNode.gCost + GetDistance(currentNode, neighbour)
					+ pathCosts[neighbour.position.x, neighbour.position.y]; // Weights

				if (totalGCost < neighbour.gCost) {
					neighbour.breadcrumbs = currentNode;
					neighbour.gCost = totalGCost;
					neighbour.hCost = GetDistance(currentNode, endNode);

					if (!OpenSet.Contains(neighbour)) OpenSet.Add(neighbour);
				}
			}
		}
		//Debug.Log("Unable to find a path");
		return null; // In case a path could not be found
	}
	private static List<Vector2Int> TracebackPath(PNode endNode, PNode startNode) {
		PNode currentNode = endNode;
		List<Vector2Int> path = new List<Vector2Int> { currentNode.position };

		while (currentNode.breadcrumbs != null) {
			currentNode = currentNode.breadcrumbs;
			path.Add(currentNode.position);
		}
		path.Reverse(); // Reverse the path, as currently it starts at the end
		return path;
	}

    #region GetDistance
    private static int GetDistance(PNode a, PNode b, int movementCost = 7, int diagonalMovementCost = 10) {
		int dX = Mathf.Abs(b.position.x - a.position.x);
		int dY = Mathf.Abs(b.position.y - a.position.y);

		return movementCost * (dX + dY) + (diagonalMovementCost - 2 * movementCost) * Mathf.Min(dX, dY);
	} // Manhattan distance adapted to diagonals
	private static int GetDistanceA(PNode a, PNode b, int movementCost = 10) {
		int dX = Mathf.Abs(b.position.x - a.position.x);
		int dY = Mathf.Abs(b.position.y - a.position.y);

		return movementCost * (dX + dY);
	} // Manhattan distance
    #endregion
    #region GetNeighbours
    private static List<PNode> GetNeighbours(Vector2Int nodePos, PNode[,] nodes, Vector2Int mapSize, bool[,] pathable) {
		//Debug.Log($"Node: [{nodePos.x},{nodePos.y}]");
		List<PNode> neighbours = new List<PNode>();
		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if (x == 0 && y == 0) continue; // Skip (0,0), it's this node

				// Check for [outside of bounds] positions, and skip those "neighbours"
				if ((nodePos.x + x) < 0 || (nodePos.x + x) >= mapSize.x) continue; // Check X axis
				if ((nodePos.y + y) < 0 || (nodePos.y + y) >= mapSize.y) continue; // Check Y axis

				// Skip non pathable nodes
				if (!pathable[nodePos.x + x, nodePos.y + y]) continue;

				// Add the Neighbour to the list
				//Debug.Log($"Neighbour: [{nodePos.x + x},{nodePos.y + y}]");
				neighbours.Add(nodes[nodePos.x + x, nodePos.y + y]);
			}
		}
		return neighbours;
	} // Surrounding neighbours
	private static List<PNode> GetNeighboursA(Vector2Int nodePos, PNode[,] nodes, Vector2Int mapSize, bool[,] pathable) {
		List<PNode> neighbours = new List<PNode>();

		for (int x = -1; x < 2; x++) {
			if (x == 0) continue; // Skip (0,Y), it's this node
			// Check for [outside of bounds] positions
			if ((nodePos.x + x) < 0 || (nodePos.x + x) >= mapSize.x) continue;

			// Skip non pathable nodes
			if (!pathable[nodePos.x + x, nodePos.y]) continue;

			neighbours.Add(nodes[nodePos.x + x, nodePos.y]);
		}
		for (int y = -1; y < 2; y++) {
			if (y == 0) continue; // Skip (X,0), it's this node
			// Check for [outside of bounds] positions
			if ((nodePos.y + y) < 0 || (nodePos.y + y) >= mapSize.y) continue;

			// Skip non pathable nodes
			if (!pathable[nodePos.x, nodePos.y + y]) continue;

			// Add the Neighbour to the list
			neighbours.Add(nodes[nodePos.x, nodePos.y + y]);
		}

		return neighbours;
	} // Adjacent neighbours
    #endregion
}
