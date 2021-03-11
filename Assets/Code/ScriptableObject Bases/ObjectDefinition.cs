using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDefinition", menuName = "New Object Definition")]
public class ObjectDefinition : ScriptableObject {
    //? Variables
    [SerializeField] private string name_id;
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Vector2Int dimensions; // Default is (1x1)


    [SerializeField] private bool isRotable; // Only matters if the object is bigger than (1x1)
    [SerializeField] private bool doesBlockPath; // If its solid, block those tiles from being pathed over

    [SerializeField] private float pathCostModifier; //If its not solid, how will it affect movement

    //? Properties
    public string Name_ID { get => name_id; }
    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
    public Vector2Int Dimensions { get => dimensions; }
    public bool IsRotable { get => isRotable; }
    public bool DoesBlockPath { get => doesBlockPath; }
    public float PathCostModifier { get => pathCostModifier; }

    //? Constructor
    public ObjectDefinition(string objName_ID, string objName, Sprite objSprite, Vector2Int objDimensions, bool isRotable = false, bool blocksPath = true, float pathCostModifier = 0) {
        this.name_id = objName_ID;
        this.name = objName;
        this.sprite = objSprite;
        this.dimensions = objDimensions;

        this.isRotable = isRotable;
        this.doesBlockPath = blocksPath;

        this.pathCostModifier = pathCostModifier;
    }
}
