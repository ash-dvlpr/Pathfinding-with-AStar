using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PNodeS {
	//? Data
	public readonly Vector2Int position;
	public PNodeS? previous;

	public readonly bool isWalkable;
    public readonly float pathCost;

	public int gCost;
	public int hCost;
	public int fCost { get => gCost + gCost; }
	
	//? Constructors
	public PNodeS(Vector2Int position, bool isWalkable, float pathCost, int gCost, int hCost, PNodeS? previous = null) {
		this.position = position;
		this.previous = previous;

		this.isWalkable = isWalkable;
		this.pathCost = pathCost;

		this.gCost = gCost;
		this.hCost = hCost;
    }

	//? Methods
	
}
