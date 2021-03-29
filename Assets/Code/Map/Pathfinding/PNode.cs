using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNode {
#nullable enable

	//? Data
	public readonly Vector2Int position;
	public PNode? breadcrumbs;
    public bool closed;

	public int gCost;
	public int hCost;
	public int fCost { get => gCost + gCost; }
	
	//? Constructors
	public PNode(Vector2Int position, int gCost = int.MaxValue, int hCost = int.MaxValue, PNode? breadcrumbs = null) {
		this.position = position;
        this.breadcrumbs = breadcrumbs;
        this.closed = false;

		this.gCost = gCost;
		this.hCost = hCost;
    }

	//? Methods


	//? Operator overloading
	// For quicker comparison
	public static bool operator == (PNode a, PNode b) {
        if ((object)a == null)
            return (object)b == null;
        return a.Equals(b);
    }
    public static bool operator != (PNode a, PNode b) {
        return !(a == b);
    }
    public override bool Equals(object other) {
        if (other == null || GetType() != other.GetType()) return false;
        var p2 = (PNode)other;
        return (this.position == p2.position);
    }
    public override int GetHashCode() {
        return
              position.GetHashCode()
            ^ gCost.GetHashCode()
            ^ hCost.GetHashCode();
    }
#nullable restore
}