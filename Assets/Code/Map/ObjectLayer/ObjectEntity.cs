using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEntity {
    //? Variables
    private ObjectLayer parentLayer;
    private Vector2Int position;

    private int id;
    private string objectName;           // //! Its fine; Stores just the name (Key of the AssetManager Dictionary) of its ObjectDefinition.
    private Orientation orientation;    // There are 4 posible orientations [0: South, 1: West, 2: North, 3: East}

    private GameObject objectGO; // This object's visual representation

    //? Properties
    public ObjectLayer ParentLayer { get => parentLayer; }
    public Vector2Int Position { get => position; }

    public int ID { get => id; }
    public string ObjectName { get => objectName; }
    public Orientation Orientation { get => orientation; }

    public GameObject ObjectGO { get => objectGO; }
    //? Constructor
    public ObjectEntity(ObjectLayer parent, Vector2Int position, ObjectDefinition objDefinition, int id, Orientation orientation = 0) {
        this.parentLayer = parent;
        this.position = position;
        this.id = id;

        this.objectName = objDefinition.Name_ID;
        this.orientation = orientation; // Default orientation is South (Looking Down)

        // Create this object's Visuals
        string name = $"{objDefinition.Name_ID} [{position.x},{position.y}]";
        Vector2 spritePos = Utils.GetBottomLeftCornerOfObject(position, objDefinition.Dimensions, orientation);

        Vector2 objectsGOpos = Utils.GridToWorldPos(spritePos, parentLayer.MapSize);
        this.objectGO = Utils.CreateSpriteGO(name, objectsGOpos, parent.ParentLayer.MapObject);
        this.objectGO.GetComponent<SpriteRenderer>().sprite = objDefinition.Sprite;
        this.objectGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    //? Methods
    public void DestroyVisual() { GameObject.Destroy(objectGO); }

    //? Events, Actions & Callbacks
}
