using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class PNodeS {
	//? Data
	public readonly Vector2Int position;
	public PNodeS? breadcrumbs;

	public int gCost;
	public int hCost;
	public int fCost { get => gCost + gCost; }
	
	//? Constructors
	public PNodeS(Vector2Int position, int gCost, int hCost, PNodeS? breadcrumbs = null) {
		this.position = position;
        this.breadcrumbs = breadcrumbs;

		this.gCost = gCost;
		this.hCost = hCost;
    }

	//? Methods
}
#nullable restore