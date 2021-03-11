using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLayer {
    //? Variables
    private Map parentMap;
    private Vector2Int mapSize;

    private GroundTile[,] groundTiles;

    //? Properties
    public GroundTile[,] GroundTiles { get => groundTiles; }
    public Vector2Int MapSize { get => mapSize; }
    public Map ParentMap { get => parentMap; }

    //? Constructor
    public GroundLayer(Map parent, Vector2Int mapSize) {
        this.parentMap = parent;
        this.mapSize = mapSize;

        this.groundTiles = new GroundTile[mapSize.x, mapSize.y]; // Array creation
        GenerateTiles();
    }

    private void GenerateTiles(bool setEmpty = false) { // By default, dont generate an empty map, unless told to.
        // Generate a noise map to be used for the generation
        // For now just use the default noise scale, should be determined by a "biome"
        float[,] noiseMap = Utils.GenerateNoiseMap(mapSize, parentMap.MapSeed);
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                //Debug.Log($"New Tile at: ({x},{y})");
                // if setEmpty, sets all tiles to empty
                if (setEmpty) { groundTiles[x, y] = new GroundTile(this, new Vector2Int(x, y), AssetManager.groundTypes["empty"]); }
                else { // else (default)
                    GroundType correspondingType = Utils.GetGroundTypeFromNoiseValue(noiseMap[x, y]);
                    groundTiles[x, y] = new GroundTile(this, new Vector2Int(x, y), correspondingType);
                }
            }
        }
    }
    public void PlaceGroundTile(Vector2Int position, GroundType newType) {
        // Check that position is inside bounds
        if (position.x < 0 || position.x >= mapSize.x) { return; } // Check X axis
        if (position.y < 0 || position.y >= mapSize.y) { return; } // Check Y axis

        // Change the corresponding tile's type
        groundTiles[position.x, position.y].SetType(newType);
    }
    public void EmptyLayer() {
        // Iterate through all tiles
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                // Set the tile at (x, y) to the "empty" type.
                groundTiles[x, y].SetType(AssetManager.groundTypes["empty"]);
            }
        }
    }
    public void Regenerate() {
        //Debug.Log("Capishe (GroundLyer)");
        // Generate a noise map to be used for the generation
        // For now just use the default noise scale, should be determined by a "biome"
        float[,] noiseMap = Utils.GenerateNoiseMap(mapSize, parentMap.MapSeed);
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                GroundType correspondingType = Utils.GetGroundTypeFromNoiseValue(noiseMap[x, y]); // Get the corresponding GroundType
                groundTiles[x, y].SetType(correspondingType); //Set the new type
            }
        }
    }
}
