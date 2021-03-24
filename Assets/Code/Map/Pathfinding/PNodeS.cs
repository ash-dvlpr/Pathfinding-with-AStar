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

	//? Operator overloading
	// For quicker comparison
	public static bool operator == (PNodeS a, PNodeS b) {
        if ((object)a == null)
            return (object)b == null;
        return a.Equals(b);
    }
    public static bool operator != (PNodeS a, PNodeS b) {
        return !(a == b);
    }
    public override bool Equals(object other) {
        if (other == null || GetType() != other.GetType()) return false;
        var p2 = (PNodeS)other;
        return (this.position == p2.position);
    }
    public override int GetHashCode() {
        return
              position.GetHashCode()
            ^ gCost.GetHashCode()
            ^ hCost.GetHashCode();
    }
}
#nullable restore