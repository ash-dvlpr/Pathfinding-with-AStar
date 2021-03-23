using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
	private static List<int> usedIDs = new List<int>();
	private static int lastID = 0;
	#region Vector2 Utils
	// "Snaps" positions to Grid Positions & Back
	public static Vector2Int WorldToGridPos(Vector2 rawPos, Vector2Int mapSize) => 
		new Vector2Int(Mathf.FloorToInt(rawPos.x) + mapSize.x / 2, Mathf.FloorToInt(rawPos.y) + mapSize.y / 2);
	public static Vector2 GridToWorldPos(Vector2 gridPos, Vector2Int mapSize) => 
		new Vector2(gridPos.x - mapSize.x / 2, gridPos.y - mapSize.y / 2);

	// "Snaps" positions inside of bounds
	public static Vector2Int DisplaceGridPosInsideBounds(Vector2Int pos, Vector2Int mapSize) => 
		new Vector2Int(Mathf.Clamp(pos.x, 0, mapSize.x - 1), Mathf.Clamp(pos.y, 0, mapSize.y - 1));
	public static Vector2 DisplaceWorldPosInsideBounds(Vector2 pos, Vector2Int mapSize) => 
		new Vector2(Mathf.Clamp(pos.x, -mapSize.x / 2, mapSize.x / 2), Mathf.Clamp(pos.y, -mapSize.y / 2, mapSize.y / 2));

	// Vector2 to Vector2Int
	public static Vector2Int V2toV2Int(Vector2 pos) => 
		new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
	#endregion
	#region GameObject Utils
	public static GameObject CreateGO(string name) {
		GameObject GO = new GameObject(name);			// Create GO
		GO.transform.position = new Vector2(0, 0);		// Places the object at (0,0)
		return GO;
	} // Creates an [empty] GameObject
	public static GameObject CreateGO(string name, GameObject parent) {
		GameObject GO = new GameObject(name);			// Create GO
		GO.transform.parent = parent.transform;			// Sets GO's Parent GameObject
		GO.transform.position = new Vector2(0, 0);		// Places the object at (0,0)
		return GO;
	} // Creates an [empty] [child] GameObject
	public static GameObject CreateSpriteGO(string name) {
		GameObject SpriteGO = new GameObject(name, typeof(SpriteRenderer));		// Create GO & Add SpriteRenderer Component
		SpriteGO.transform.position = new Vector2(0, 0);						// Places the object at (0,0)
		return SpriteGO;
	} // Creates an [empty] Sprite
	public static GameObject CreateSpriteGO(string name, Vector2 position) {
		GameObject SpriteGO = new GameObject(name, typeof(SpriteRenderer));		// Create GO & Add SpriteRenderer Component
		SpriteGO.transform.position = position;									// Places the GO at (position)
		return SpriteGO;
	} // Creates an [empty] Sprite
	public static GameObject CreateSpriteGO(string name, Vector2 position, GameObject parent) {
		GameObject SpriteGO = new GameObject(name, typeof(SpriteRenderer));		// Create GO & Add SpriteRenderer Component
		SpriteGO.transform.parent = parent.transform;							// Sets GO's Parent GameObject
		SpriteGO.transform.position = position;									// Places the GO at (position)
		return SpriteGO;
	} // Creates an [empty] [child] Sprite at (position)
	public static GameObject CreateQuad(string name, GameObject parent, Vector2 centerPos, Vector2Int size) {
		// Make the empty game object to hold this
		GameObject QuadGO = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));	// Also give it the components needed

		// Position the GameObject and set its parent
		QuadGO.transform.position = centerPos;
		QuadGO.transform.parent = parent.transform;

		// Make the mesh's data
		Vector3[] vertices = new Vector3[] { // Corner positions scale scale with size
			new Vector2(centerPos.x - size.x / 2, centerPos.y - size.y / 2),  // Lower Left corner
			new Vector2(centerPos.x - size.x / 2, centerPos.y + size.y / 2),  // Upper Left corner
			new Vector2(centerPos.x + size.x / 2, centerPos.y - size.y / 2),  // Lower Right corner
			new Vector2(centerPos.x + size.x / 2, centerPos.y + size.y / 2)   // Upper Right corner
		};
		int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };

		// Get the Quad's mesh
		Mesh mesh = QuadGO.GetComponent<MeshFilter>().mesh;

		// Apply the data to the mesh
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		// Return the Quad's GameObject
		return QuadGO;
	} // Creates an [empty] [child] Quad, of (size) size, centered at (position)
	#endregion

	// Gets the bottom left corner of an "area"
	public static Vector2Int GetBottomLeftCornerOfObject(Vector2Int position, Vector2Int objectSize, Orientation orientation) {
		// Depends on orientation
		switch (orientation) {
			case Orientation.South: // Looking South
				return position;
			case Orientation.West: // Looking West
				return new Vector2Int(position.x, position.y - (objectSize.x - 1));
			case Orientation.North: // Looking North
				return new Vector2Int(position.x - (objectSize.x - 1), position.y - (objectSize.y - 1));
			case Orientation.East: // Looking Easts
				return new Vector2Int(position.x - (objectSize.y - 1), position.y);
			default: return Vector2Int.zero; // Non legal orientation
		}
	}

	// Procedural generation stuff
	public static float[,] GenerateNoiseMap(Vector2Int mapSize, string seed, float scale = 75) {
		float[,] NoiseMap = new float[mapSize.x, mapSize.y]; // Initialize the array;

		// Calculate randomized offsets with the seed.
		System.Random RNG = new System.Random(seed.GetHashCode());
		int xOffset = RNG.Next(0, 999);
		int yOffset = RNG.Next(0, 999);

		// Cycle though all positions of the map. Determined by the mapSize.
		for (int x = 0; x < mapSize.x; x++) {
			for (int y = 0; y < mapSize.y; y++) {
				//? Calculate noise position coordinates
				// Conver from int to a value from 0 to 1, because perlin noise repeats at whole numbers
				// Multiply by the scale factor, to, well, scale the noise. Bigger scales retain less detail; Its loke zooming out
				// To not get the same thing every time, you offset the values by a value. That is how you "apply" a seed.
				float px = (float)x / scale;
				float py = (float)y / scale;
				px += xOffset;
				py += yOffset;

				NoiseMap[x, y] = Mathf.PerlinNoise(px, py);

				//Debug.Log($"{px}, {py} = {NoiseMap[x, y]}"); //! Debugging
			}
		}
		// Retunrs the final map
		return NoiseMap;
	}
	public static GroundType GetGroundTypeFromNoiseValue(float noise) {
		// Perlin noise values tend to range from 0 to 1
		switch (noise) {
			case float n when (n >= .8f):
				// Return grass
				return AssetManager.groundTypes["stone"];
			case float n when (n >= .7f):
				// Return gravel
				return AssetManager.groundTypes["gravel"];
			case float n when (n >= .5f):
				// Return grass
				return AssetManager.groundTypes["grass"];
			case float n when (n >= .45f):
				// Return dirt
				return AssetManager.groundTypes["dirt"];
			case float n when (n >= .38f):
				// Return sand
				return AssetManager.groundTypes["sand"];
			case float n when (n >= .34f):
				// Return water
				return AssetManager.groundTypes["water"];
			case float n when (n <= 0 || n >= 0f):
				// Return shallow water
				return AssetManager.groundTypes["shallow_water"];
			default:
				return AssetManager.groundTypes["empty"];
		}
	} // TODO: This is harcoded; Should be able to pass on a Biome (template) and parse it

    #region ID Generation
    public static int GenerateRandomID() {
		int newID;
		do { // Generate a new ID
			newID = (int)Random.Range(1, int.MaxValue);
			//Debug.Log(newOID);//! Debug
		} while (usedIDs.Contains(newID)); // Untill the ID is not unique, dont stop

		//If the newID is unique, add it to the used list & return it
		usedIDs.Add(newID);
		return newID;
	} // Generates a [unique] [random] ID
	public static int GenerateNewID() { // Version that creates IDs with the lowest values
		int newID = lastID;
		do { // Add 1 to the last used ID
			newID += 1;
			//Debug.Log(newOID);//! Debug
		} while (usedIDs.Contains(newID)); // Untill the ID is not unique, dont stop

		//If the newID is unique, add it to the used list & return it
		usedIDs.Add(newID);
		return newID;
	} // Generates a [unique] [incremental] ID (starts counting from 1)
    #endregion
}

public enum Orientation : byte {
	South = 0,
	West = 1,
	North = 2,
	East = 3
}