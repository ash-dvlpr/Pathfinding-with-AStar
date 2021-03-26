using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDataLayer {
    //? Variables
    private Map parentMap;
    private Vector2Int mapSize;

    private bool[,] isWalkable;    // Affected by both the ground tile type & objects that block the path
    private int[,] pathCost;       // Affected by the ground tile type

    //? Properties
    public Map ParentMap { get => parentMap; }
    public Vector2Int MapSize { get => mapSize; }
    public bool[,] IsWalkable { get => isWalkable; }
    public int[,] PathCost { get => pathCost; }

    //? Constructor
    public PathDataLayer(Map parent, Vector2Int mapSize) {
        this.parentMap = parent;
        this.mapSize = mapSize;

        // "Nodes" are a combination of 2 values, a boolean for checking if that position can be pathed though, and a path cost
        this.isWalkable = new bool[mapSize.x, mapSize.y];
        this.pathCost = new int[mapSize.x, mapSize.y];
        InitializeNodes();
    }

    //? Methods
    private void InitializeNodes() {
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                // Its gonna be walkable If the ground tile is walkable & If not blocked by an object
                isWalkable[x, y] = IsPositionPathable(new Vector2Int(x, y));

                // Set the path cost values
                pathCost[x, y] = AssetManager.groundTypes[parentMap.GroundLayer.GroundTiles[x, y].TypeName].PathCost;
            }
        }
    }
    public void EmptyData() {
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                isWalkable[x, y] = false;
                pathCost[x, y] = 0;
            }
        }
    }
    public void UpdateData() {
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                isWalkable[x, y] = IsPositionPathable(new Vector2Int(x, y));
                pathCost[x, y] = AssetManager.groundTypes[parentMap.GroundLayer.GroundTiles[x, y].TypeName].PathCost;
            }
        }
    } // Updates the information for the whole layer at once
    // public void UpdateNode(Vector2Int position) {} // Gets the information from the layers directly
    // public void UpdateNode(Vector2Int position, bool isWalkable, int pathCost) {} // You have to manualy pass the values
    public bool IsPositionPathable(Vector2Int position) {
        if (!AssetManager.groundTypes[parentMap.GroundLayer.GroundTiles[position.x, position.y].TypeName].IsWalkable) return false;
        if (parentMap.ObjectLayer.IsTileEmpty(position)) return true; // Theres no object there, pathable
        return !parentMap.ObjectLayer.IsPathBlocked(position);
    }
}
