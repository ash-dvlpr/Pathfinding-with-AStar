using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {
    //? Variables
    //private PathDataLayer pathingData;
    public readonly Vector2Int position;

    public readonly bool isWalkable;
    public readonly float pathCost;

    public float gCost = 0; // Total cost to reach this cell from the start node
    public float hCost = 0; // Total distance to goal / end node
    public float fCost { get => gCost + hCost; } // Total cost of this node

    public PathNode cameFrom;

    //? Properties

    //? Constructor
    public PathNode(PathDataLayer pathingData, Vector2Int position, PathNode cameFrom = null) {
        //this.pathingData = pathingData;
        this.position = position;

        // Get the info from the pathing data layer linked to a map
        this.isWalkable = pathingData.IsWalkable[position.x, position.y];
        this.pathCost = pathingData.PathCost[position.x, position.y];

        this.cameFrom = (cameFrom == null) ? this : cameFrom;
    }

    public List<PathNode> GetNeighbours(PathDataLayer pathingData) {
        List<PathNode> neighbours = new List<PathNode>();

        for (int x = -1; x < 2; x++) {
            for (int y = -1; y < 2; y++) {
                if (x == 0 && y == 0) continue; // Skip (0,0), it's this node

                // Check for outside of bounds, and skip those "neighbours"
                if (position.x + x < 0 || position.x + x >= pathingData.MapSize.x) continue; // Check X axis
                if (position.y + y < 0 || position.y + y >= pathingData.MapSize.x) continue; // Check Y axis

                // Add the Neighbour to the list
                neighbours.Add(new PathNode(pathingData, new Vector2Int(position.x + x, position.y + y)));
            }
        }

        return neighbours;
    }

    
}
