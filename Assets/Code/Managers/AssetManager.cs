using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour {
    //? Ground Tiles
    public static Dictionary<string, GroundType> groundTypes = new Dictionary<string, GroundType>();
    public static List<string> groundTypesKeyList = new List<string>(); // Create keyList

    //? Object Definitions
    public static Dictionary<string, ObjectDefinition> objectDefinitions = new Dictionary<string, ObjectDefinition>();
    public static List<string> objectDefinitionsKeyList = new List<string>(); // Create keyList

    void Start() {
        #region GroundTiles
        // Auto-loads all GroundType Scriptable Objects into a static global dictionary
        foreach (var type in (Resources.LoadAll<GroundType>("ScriptableObjects/GroundTypes")) ) {
            groundTypes.Add(type.Name_ID, type);
            //! Testing
            //Debug.Log($"Added type. Name: {type.name}, Color: {type.TileColor}, Walkable?: {type.IsWalkable}, Cost: {type.PathCost}");
        }
        // Pre-creates a list of all GroundType keys
        var gtDKeys = groundTypes.Keys; // Get all keys
        foreach (string key in gtDKeys) { 
            groundTypesKeyList.Add(key);
            //! Testing
            //Debug.Log(key);
        }
        #endregion
        #region ObjectDefinitions
        // Auto-loads all ObjectDefinition Scriptable Objects into a static global dictionary
        foreach ( var objDef in (Resources.LoadAll<ObjectDefinition>("ScriptableObjects/ObjectDefinitions")) ) {
            objectDefinitions.Add(objDef.Name_ID, objDef);
            //! Testing
            //Debug.Log($"Added Object. Name: {objDef.Name}, Dimensions: [{objDef.Dimensions.x}, {objDef.Dimensions.y}], Rotable?: {objDef.IsRotable}, Solid?: {objDef.IsSolid}, Cost Modifier: {objDef.PathCostModifier}");
        }
        // Pre-creates a list of all GroundType keys
        var odDKeys = objectDefinitions.Keys; // Get all keys
        foreach (string key in odDKeys)
        {
            objectDefinitionsKeyList.Add(key);
            //! Testing
            //Debug.Log(key);
        }
        #endregion
    }
}