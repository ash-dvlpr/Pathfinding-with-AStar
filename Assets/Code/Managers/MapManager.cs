using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MapManager : MonoBehaviour {
    //? Variables
    [System.NonSerialized] public Map map;
    private GameObject mapGO;
    private GameObject groundLayerSprite;

    public Vector2Int mapSize = new Vector2Int(10, 10); //! Testing
    public string defaultSeed = "1234567890";
    private bool regenerating = false;
    //public bool randomizeOnStart = true; //! Testing

    void Start() {
        mapGO = Utils.CreateGO("Map"); // Create the Map's GO
        map = new Map(mapSize, mapGO, defaultSeed); // Create a Map
        CreateVisuals(map); //Creates the visuals for that map
    }

    void Update() {
    }

    void CreateVisuals(Map map) { // All in one method
        // GroundLayer
        CreateGroundLayerTexture(mapGO, map.GroundLayer);                       // Create a texture for the ground

        // ObjectLayer
        GameObject objectLayerGO = Utils.CreateChildGO("ObjectLayer", mapGO);   // Create the ObjectLayer's GO

        // PathingLayer
    }
    #region GroundLayer (Individual Sprites - Old/Deprecated)
    void CreateGroundLayerSprites(GameObject layerGO, GroundLayer layer) {
        // Loop though all the tiles: Add its corresponding GO, Set Sprite, Link to callback
        for (int x = 0; x < layer.MapSize.x; x++) {
            for (int y = 0; y < layer.MapSize.y; y++) {
                // Do things for this position (x,y)
                GroundTile tile = layer.GroundTiles[x, y]; //* Gets the tile for this iteration (x,y)
                string name = $"GroundTile [{tile.Position.x}, {tile.Position.y}]"; //* Generates a name for the GO
                //name += $" - Type: {tile.TypeName}"; //! Testing

                //* Calculates the Sprite GO's Position (Centered)
                Vector2 position = Utils.GridToWorldPos(tile.Position, layer.MapSize);
                GameObject tileSpriteGO = Utils.CreateSpriteGO(name, position, layerGO); //* Add Sprite GO

                //* Initial Sprite status, will be modified by events
                tileSpriteGO.GetComponent<SpriteRenderer>().sprite = AssetManager.groundTypes[tile.TypeName].Sprite; //! Referencing the Sprite from the Asset Manager
                //tileSpriteGO.GetComponent<SpriteRenderer>().color = AssetManager.groundTypes[tile.TypeName].TileColor; //! Testing Colors
                tile.RegisterTypeChangeCallback((_tile) => { OnGroundTypeChange(_tile, tileSpriteGO); });
            }
        }
    }
    // TODO: DestroyGroundLayerVisuals(GameObject layerGO, GroundLayer layer)
    void OnGroundTypeChange(GroundTile affectedTile, GameObject affectedTileSpriteGO) {
        //? Basically we are updating the Tile's GameObject to have the corresponding sprite
        try {
            affectedTileSpriteGO.GetComponent<SpriteRenderer>().sprite = AssetManager.groundTypes[affectedTile.TypeName].Sprite; //! Referencing the Sprite form the Asset Manager
            //affectedTileSpriteGO.GetComponent<SpriteRenderer>().color = AssetManager.groundTypes[affectedTile.TypeName].TileColor; //! Testing Colors
        } catch (System.Exception) {
            Debug.LogError("Non suported ground tile"); throw;
        }
    }
    #endregion
    #region GroundLayer (Texture)
    void CreateGroundLayerTexture(GameObject parent, GroundLayer layer, string name = "GroundLayer[Unified]", int spriteRes = 32) {
        // Create a Quad, and a texture for it
        groundLayerSprite = Utils.CreateSpriteGO(name, Vector2.zero, parent);

        Vector2Int textureRes = new Vector2Int(layer.MapSize.x * spriteRes, layer.MapSize.y * spriteRes); // Dimensions for the texture depend on sprite resolution
        Texture2D groundLayerTexture = new Texture2D(textureRes.x, textureRes.y);   // Create the texture

        // Loop though all the tiles: Get the tile, get its sprite, dump it inside the bigger texture
        for (int x = 0; x < layer.MapSize.x; x++) {
            for (int y = 0; y < layer.MapSize.y; y++) {
                // Do things for this position (x,y)
                GroundTile tile = layer.GroundTiles[x, y]; //* Gets the tile for this iteration (x,y)
                Color[] tileSprite = AssetManager.groundTypes[tile.TypeName].Sprite.texture.GetPixels();

                // Dump the tiles info inside the unified texture
                groundLayerTexture.SetPixels(x * spriteRes, y * spriteRes, spriteRes, spriteRes, tileSprite);

                // Object linking
                tile.RegisterTypeChangeCallback((_tile) => { OnGroundTypeChangeUnifiedTexture(_tile, groundLayerSprite, spriteRes); });
            }
        }

        // Apply changes to the texture
        groundLayerTexture.Apply();
        groundLayerTexture.filterMode = FilterMode.Point;

        // Apply the texture to the GameObject
        Sprite newSprite = Sprite.Create(groundLayerTexture, new Rect(0, 0, groundLayerTexture.width, groundLayerTexture.height), Vector2.one * .5f, textureRes.x / layer.MapSize.x);
        groundLayerSprite.GetComponent<SpriteRenderer>().sprite = newSprite;

        // TODO: linking every Tile with the OnGroundTypeChangeQuad method
    }
    // TODO: DestroyGroundLayerVisuals(GameObject layerGO, GroundLayer layer)
    void OnGroundTypeChangeUnifiedTexture(GroundTile affectedTile, GameObject layerSpriteGO, int spriteRes = 32) {
        try {
            if (regenerating) { return; } // Avoid updating the whole map just for a tile when regenerating the whole map.
            // Get the layer's texture
            SpriteRenderer layerRenderer = layerSpriteGO.GetComponent<SpriteRenderer>();
            // Get the tile's new sprite texture
            //Debug.Log($"Position: {affectedTile.Position}, {affectedTile.TypeName}"); //! Debugging
            Color[] newTileSprite = AssetManager.groundTypes[affectedTile.TypeName].Sprite.texture.GetPixels();
            // Set the corresponding pixels of the layer's texture to that of the tile's texture
            layerRenderer.sprite.texture.SetPixels(affectedTile.Position.x * spriteRes, affectedTile.Position.y * spriteRes, spriteRes, spriteRes, newTileSprite);
            // Apply the changes
            layerRenderer.sprite.texture.Apply();
            //layerRenderer.sprite = Sprite.Create(layerTexture, new Rect(0, 0, layerTexture.width, layerTexture.height), Vector2.one * .5f, layerTexture.width / affectedTile.ParentLayer.MapSize.x); ;
        } catch (System.Exception) {
            Debug.LogError("Non suported ground type"); throw;
        }
    }
    void UpdateGroundLayerUnfiedTexture(GroundLayer layer, int spriteRes = 32) {
        // Get the layer's sprite & texture
        GameObject layerSpriteGO = mapGO.transform.Find("GroundLayer[Unified]").gameObject;
        SpriteRenderer layerRenderer = layerSpriteGO.GetComponent<SpriteRenderer>();
        Texture2D layerTexture = layerRenderer.sprite.texture;
        // Loop though all the tiles: Get the tile, get its sprite, dump it inside the bigger texture
        for (int x = 0; x < layer.MapSize.x; x++) {
            for (int y = 0; y < layer.MapSize.y; y++) {
                GroundTile tile = layer.GroundTiles[x, y]; //* Gets the tile for this iteration (x,y)
                Color[] tileSprite = AssetManager.groundTypes[tile.TypeName].Sprite.texture.GetPixels();

                // Dump the tiles info inside the unified texture
                layerTexture.SetPixels(x * spriteRes, y * spriteRes, spriteRes, spriteRes, tileSprite);
            }
        }

        // Apply changes
        layerTexture.Apply();
    }
    #endregion

    #region ObjectLayer
    //void CreateObjectLayerVisuals() { }
    #endregion

    #region PathingLayer
    #endregion

    public void RegenerateMap(string newSeed = "") {
        regenerating = true; // Very important to avoid huge memory and CPU usage
        if (newSeed == "") { // If the string is empty, get a "random" seed
            //Debug.Log("You didn't input a seed"); //! Debugging
            System.Random RNG = new System.Random();
            newSeed = RNG.Next(0, int.MaxValue).ToString();
            //Debug.Log($"Final New Seed: {newSeed}"); //! Debugging
        }
        map.RegenerateMap(newSeed);
        UpdateGroundLayerUnfiedTexture(map.GroundLayer);
        regenerating = false;
    }
}