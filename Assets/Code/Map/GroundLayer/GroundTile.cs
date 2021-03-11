using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile {
    //? Variables
    private GroundLayer parentLayer;
    private Vector2Int position;
    private string typeName; //! Its fine; Stores just the name (key of the AssetManager Dictionary) of its GroundType, better for memory usage.

    //? Properties
    public GroundLayer ParentLayer { get => parentLayer; }
    public Vector2Int Position { get => position; }
    public string TypeName { get => typeName; } // Use the dedicated method to change the type.

    //? Constructor
    public GroundTile(GroundLayer parent, Vector2Int position, GroundType type) {
        this.parentLayer = parent;
        this.position = position;
        this.typeName = type.Name_ID;
    }

    //? Methods
    public void SetType(GroundType newType) {  // When you change the type, graphics gotta get updated. Calls event.
        if (typeName != newType.Name_ID) { //! Prevents callbacks when nothing really changed
            //Debug.Log("Stop touching me!"); //! Testing
            typeName = newType.Name_ID;    // Assigns value (like if it was a set property)
            if (TypeChanged != null) {  //! Prevents overwritting the callback
                TypeChanged(this);      //  Creates the callback
            }
        }
    }

    //? Events, Actions & Callbacks
    Action<GroundTile> TypeChanged; // Acción
    public void RegisterTypeChangeCallback(Action<GroundTile> callback) => TypeChanged += callback;   // Registers a callback
    public void UnRegisterTypeChangeCallback(Action<GroundTile> callback) => TypeChanged += callback; // UnRegisters a callback
}
