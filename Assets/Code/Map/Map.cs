using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {
    //? Variables
    private string mapSeed;
    private Vector2Int mapSize;

    private GameObject mapObject;

    //* Layers
    //! All maps are a compilation of layers + magic
    private GroundLayer groundLayer;
    private ObjectLayer objectLayer;
    private PathDataLayer pathDataLayer;

    //? Properties
    public Vector2Int MapSize { get => mapSize; }
    public string MapSeed { get => mapSeed; }
    public GameObject MapObject { get => mapObject; }

    public GroundLayer GroundLayer { get => groundLayer; }
    public ObjectLayer ObjectLayer { get => objectLayer; }
    public PathDataLayer PathDataLayer { get => pathDataLayer; }

    //? Constructor
    public Map(Vector2Int mapSize, GameObject mapObject, string seed = "1234567890") {
        this.mapSize = mapSize;
        this.mapSeed = seed;

        this.mapObject = mapObject;

        this.groundLayer = new GroundLayer(this, mapSize);      // Creates Ground Layer
        this.objectLayer = new ObjectLayer(this, mapSize);      // Creates Object Layer
        this.pathDataLayer = new PathDataLayer(this, mapSize);  // Creates PathDataLayer
    }

    //? Methods
    public void RegenerateMap(string newSeed = "") {
        //Debug.Log("Ok. (Map)");
        EmptyMap(); // Empty the map
        if (newSeed == "empty") { return; } // If the seed is literally the word: "empty", don't do anything else. Don't even change the seed

        if (newSeed != "") { this.mapSeed = newSeed; } // Unless you pass in a new seed, don't change the current one, use old one

        //TODO: Do the same for every layer that needs to have its contents regenerated (mostly things that have map generation)
        this.groundLayer.Regenerate();
        this.pathDataLayer.UpdateData();
    }
    public void EmptyMap() {
        // For clearing the whole map, clear ALL relevant information contained in ALL layers
        this.groundLayer.EmptyLayer();
        this.objectLayer.EmptyLayer();
        this.pathDataLayer.EmptyData();
    }
}