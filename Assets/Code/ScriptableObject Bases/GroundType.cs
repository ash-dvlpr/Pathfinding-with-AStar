using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundType", menuName = "New GroundType")]
public class GroundType : ScriptableObject {
    //? Variables
    [SerializeField] private string name_id;
    [SerializeField] private new string name;
    [SerializeField] private Sprite sprite;

    [SerializeField] private bool isWalkable = true;
    [SerializeField] private int pathCost;

    //? Properties
    public string Name_ID { get => name_id; }
    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    public bool IsWalkable { get => isWalkable; }
    public int PathCost{ get => pathCost; }

    //? Constructor
    public GroundType(string typeNameID, string typeName, Sprite tileSprite, bool isWalkable, int pathCost)
    {
        this.name_id = typeNameID;
        this.name = typeName;
        this.sprite = tileSprite;

        this.isWalkable = isWalkable;
        this.pathCost = pathCost;
    }
}