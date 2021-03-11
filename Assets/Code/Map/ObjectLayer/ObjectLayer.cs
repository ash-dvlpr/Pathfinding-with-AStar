using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLayer {
    //? Variables
    private Map parentMap;
    private Vector2Int mapSize;

    private ObjectEntity[,] objectEntities;         // The objects per se
    private Dictionary<int, Vector2Int> objectIDs;  // A dictionary that links an ID with its corresponding object

    // I'm using "int?" because it adds "null" as a valid value. Needed for clearing the array / identifying empty positions.
    private int?[,] objectSpace;                     // A grid of IDs of the representations of the dimensions of the Objects

    //? Properties
    public Map ParentLayer { get => parentMap; }
    public Vector2Int MapSize { get => mapSize; }

    //? Constructor
    public ObjectLayer(Map parent, Vector2Int mapSize) {
        this.parentMap = parent;
        this.mapSize = mapSize;

        this.objectIDs = new Dictionary<int, Vector2Int>();             // Dictionary creation
        this.objectEntities = new ObjectEntity[mapSize.x, mapSize.y];   // Array creation
        this.objectSpace = new int?[mapSize.x, mapSize.y];              // Array creation
        InitializeObjectSpace();
    }


    //? Methods
    #region Important Stuff (Creation/Emptying of the layer)
    private void InitializeObjectSpace() {              // Initializes the whole array with null
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                objectSpace[x, y] = null;
            }
        }
    }
    public void EmptyLayer() {
        // Destroy all the visuals
        foreach (var id in objectIDs.Keys) {
            objectEntities[objectIDs[id].x, objectIDs[id].y].DestroyVisual();
        }

        // Clear all IDs and references to their objects
        objectIDs.Clear();

        // For all positions of the map
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                objectEntities[x, y] = null;    // Clear the ObjectEntity
                objectSpace[x, y] = null;       // Clear the ID stored on the ObjectSpace
            }
        }
    }
    #endregion
    // Getting Info about the objects
    private Vector2Int GetObjectEntityPos(int id) { return objectIDs[id]; }
    private int? GetObjectID(Vector2Int position) {
        return objectSpace[position.x, position.y];
    }
    private ObjectDefinition GetObjectDef(int objID) {
        Vector2Int entityPos = objectIDs[objID];
        return AssetManager.objectDefinitions[objectEntities[entityPos.x, entityPos.y].ObjectName];
    }
    #region Creating / Destroying object entities
    public void PlaceObject(ObjectDefinition obj, Vector2Int position, Orientation orientation = 0) {
        if (!IsInsideBounds(position, obj.Dimensions, orientation)) { return; } // If the object would end up out of bounds, stop
        if (CollidesWithAnything(position, obj.Dimensions, orientation)) { return; } // If the object collides with anything, stop
        // If everything is ok, continue

        if (obj.IsRotable == false) { orientation = 0; } // If the object can't be rotated, set default rotation
        Vector2Int fillStart = Utils.GetBottomLeftCornerOfObject(position, obj.Dimensions, orientation);
        Vector2Int fillEnd = fillStart + obj.Dimensions;
        int newObjID = Utils.GenerateNewID();

        // Fill the object space
        FillObjectSpace(fillStart, fillEnd, newObjID);


        // TODO: Create Visual GameObject/Sprite, its done by the object itself
        // Place the object at the objects array
        objectEntities[position.x, position.y] = new ObjectEntity(this, position, obj, newObjID, orientation);

        // Add the object to the Dictionary
        objectIDs.Add(newObjID, position);

        // Update the Pathing Data
        parentMap.PathDataLayer.UpdateData();
    }
    public void DestroyObject(Vector2Int position) {
        // Check that position is inside bounds
        if (position.x < 0 || position.x >= mapSize.x) { return; } // Check X axis
        if (position.y < 0 || position.y >= mapSize.y) { return; } // Check Y axis

        // Get that object's ID
        int? objID = GetObjectID(position);
        if (objID == null) { return; } // Theres no object there, nothing to do

        // Get the ObjectEntity's position / Object's origin point
        Vector2Int objPos = objectIDs[(int)objID];

        // TODO: Destroy Visual GameObject/Sprite
        objectEntities[objPos.x, objPos.y].DestroyVisual();

        // Clear the space it occupied
        Vector2Int fillStart = Utils.GetBottomLeftCornerOfObject(objPos, AssetManager.objectDefinitions[objectEntities[objPos.x, objPos.y].ObjectName].Dimensions, objectEntities[objPos.x, objPos.y].Orientation);
        Vector2Int fillEnd = fillStart + AssetManager.objectDefinitions[objectEntities[objPos.x, objPos.y].ObjectName].Dimensions;
        FillObjectSpace(fillStart, fillEnd, null); // Fill with null

        // Delete the reference to its ID
        objectIDs.Remove((int)objID);

        // Delete the ObjectEntity
        objectEntities[objPos.x, objPos.y] = null;

        // Update the Pathing Data
        parentMap.PathDataLayer.UpdateData();
    }
    #endregion
    #region Sanity Checks
    public bool IsTileEmpty(Vector2Int position) => objectSpace[position.x, position.y] == null;
    public bool IsPathBlocked(Vector2Int position) {
        if (IsTileEmpty(position)) return false;
        return GetObjectDef((int)GetObjectID(position)).DoesBlockPath; // Solid Objects block paths
    }
    private bool IsInsideBounds(Vector2Int position, Vector2Int objectSize, Orientation orientation) {
        // Check if the origin position is itself inside of bounds
        if (position.x < 0 || position.x >= mapSize.x) { return false; } // Check X axis
        if (position.y < 0 || position.y >= mapSize.y) { return false; } // Check Y axis
        // If the origin position itself is inside of bounds, continue

        if (objectSize == Vector2Int.one) { return true; } // A (1x1) object will always fit inside the grid

        // Check if the object itself fits inside the grid
        if (objectSize.x > mapSize.x || objectSize.x > mapSize.y) { return false; } // The Object is too big
        if (objectSize.y > mapSize.y || objectSize.y > mapSize.x) { return false; } // The Object is too big

        // For variable size objects
        int maxX = mapSize.x - 1, maxY = mapSize.y - 1; // The maximum X and Y at which you can place a (1x1) object
        int minX = 0, minY = 0;                         // The minimun X and Y at which you can place a (1x1) object

        switch (orientation) {
            case Orientation.South: // Looking South
                maxX -= objectSize.x - 1;
                maxY -= objectSize.y - 1;
                break;
            case Orientation.West: // Looking West
                minY = objectSize.x - 1;
                maxX -= objectSize.y - 1;
                break;
            case Orientation.North: // Looking North
                minX = objectSize.x - 1;
                minY = objectSize.y - 1;
                break;
            case Orientation.East: // Looking Easts
                minX = objectSize.y - 1;
                maxY -= objectSize.x - 1;
                break;
            default: return false; // Non legal orientation
        }

        // Check if the object origin position is inside the range.
        if (position.x < minX || position.x > maxX) { return false; } // If the X axis is not inside the range, the object will end up out of bounds
        if (position.y < minY || position.y > maxY) { return false; } // If the Y axis is not inside the range, the object will end up out of bounds

        // If the position passes all tests, the object will be inside bounds
        return true;
    }
    private bool CollidesWithAnything(Vector2Int position, Vector2Int objectSize, Orientation orientation) {
        // Correct the starting point for the Fill Check
        Vector2Int startPos = Utils.GetBottomLeftCornerOfObject(position, objectSize, orientation);
        // Set the end position for the Fill Check
        Vector2Int endPos = startPos + objectSize;

        // Check the area for other objects
        for (int x = startPos.x; x < endPos.x; x++) {
            for (int y = startPos.y; y < endPos.y; y++) {
                if (objectSpace[x, y] != null) { return true; } // If you find any set IDs, then there's an object there
            }
        }

        // If there were no already set IDs, this space is clear, so no collisions
        return false;
    }
    #endregion
    private void FillObjectSpace(Vector2Int startPos, Vector2Int endPos, int? ID) {
        for (int x = startPos.x; x < endPos.x; x++) {
            for (int y = startPos.y; y < endPos.y; y++) {
                objectSpace[x, y] = ID;
            }
        }
    }
}