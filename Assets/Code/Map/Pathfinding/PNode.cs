using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class PNode {
    //? Variables
    //private PathDataLayer pathingData;
    public readonly Vector2Int position;

    public readonly bool isWalkable;
    public readonly float pathCost;

    public float gCost = 0; // Total cost to reach this cell from the start node
    public float hCost = 0; // Total distance to goal / end node
    public float fCost { get => gCost + hCost; } // Total cost of this node

    public PNode cameFrom;

    //? Properties

    //? Constructor
    public PNode(PathDataLayer pathingData, Vector2Int position, PNode cameFrom = null) {
        //this.pathingData = pathingData;
        this.position = position;

        // Get the info from the pathing data layer linked to a map
        this.isWalkable = pathingData.IsWalkable[position.x, position.y];
        this.pathCost = pathingData.PathCost[position.x, position.y];

        this.cameFrom = (cameFrom == null) ? this : cameFrom;
    }

    public List<PNode> GetNeighbours(PathDataLayer pathingData) {
        List<PNode> neighbours = new List<PNode>();

        for (int x = -1; x < 2; x++) {
            for (int y = -1; y < 2; y++) {
                if (x == 0 && y == 0) continue; // Skip (0,0), it's this node

                // Check for outside of bounds, and skip those "neighbours"
                if (position.x + x < 0 || position.x + x >= pathingData.MapSize.x) continue; // Check X axis
                if (position.y + y < 0 || position.y + y >= pathingData.MapSize.y) continue; // Check Y axis

                // Add the Neighbour to the list
                neighbours.Add(new PNode(pathingData, new Vector2Int(position.x + x, position.y + y), this));
            }
        }

        return neighbours;
    }

    //? Operator overriding for comparing 2 nodes
    // Just taking into consideration the positions, as other values may vary
    public static bool operator == (PNode a, PNode b) {
        if ((object)a == null)
            return (object)b == null;

        return a.Equals(b);
    }
    public static bool operator != (PNode a, PNode b) {
        return !(a == b);
    }
    public override bool Equals(object other) {
        if (other == null || GetType() != other.GetType()) return false;

        var p2 = (PNode)other;
        return (this.position == p2.position);
    }

    public override int GetHashCode() {
        return this.position.GetHashCode() 
            ^ isWalkable.GetHashCode()
            ^ pathCost.GetHashCode()
            ^ gCost.GetHashCode()
            ^ hCost.GetHashCode()
            ^ cameFrom.GetHashCode();
    }
}
