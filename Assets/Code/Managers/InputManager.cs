using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {
    //? References to other Managers (Scripts)
    [Header("References to Managers")]
    public MapManager MapManager;
    public GameObject uiCanvas;

    //? Buttons and Text fields
    [Header("Buttons, Text fields & Others")]
    public InputField SeedInputField;
    public GameObject ToolPreview_Image;
    private Image previewImage;
    public GameObject ToolPreview_ToolName;
    public GameObject ToolPreview_SelectedName;
    private Text previewSelectedText;
    public GameObject ToolPreview_PlaceTooltip;

    //? Variables
    private float deltaTimeScaler = 60;
    [Header("Camera Stuff")]
    public GameObject CameraHandle;
    private Vector2 cameraTarget = Vector2.zero;
    private Camera gameCamera;

    [Header("Camera Panning")]
    public float panningSpeed = 1;
    public float minPanningSpeed = 0.5f;
    public float maxPanningSpeed = 1.5f;
    public float panSpeedModifier = 0.1f;
    [Header("Camera Zooming")]
    public float cameraZoom = 100;
    public float minZoom = 30f;
    public float maxZoom = 100f;
    public float zoomModifier = 10f;

    [Header("Other")]
    public GameObject pathing_StartNode;
    public GameObject pathing_EndNode;
    private ToolMode currentMode = ToolMode.Pathfinding; // Default mode is the Pathfinding Mode
    private int selectedIndex = 1;
    private Orientation placingOrientation = Orientation.South;

    //? Enums
    private enum ToolMode {
        Pathfinding = 0,
        Terrain = 1,
        Objects = 2
    }

    private void Start() {
        gameCamera = CameraHandle.GetComponentInChildren<Camera>();
        previewImage = ToolPreview_Image.GetComponent<Image>();
        previewSelectedText = ToolPreview_SelectedName.GetComponent<Text>();
        ChangeMode(ToolMode.Pathfinding);
        //Application.targetFrameRate = 10; //! Debugging
    }
    void Update() {
        ManageKeyboardInputs();
        ManageMouseInputs();
        UpdatePlacePreviewPosition();
    }

    //? Custom Methods
    public void Regenerate() {
        //Debug.Log($"Your Input: {newSeed}"); //! Debuging
        string inputSeed = SeedInputField.text; // Sets the seed to that of the InputField
        MapManager.RegenerateMap(inputSeed);    // Tells the map manager to regenerate the map
    }
    private void ToggleUI() { uiCanvas.SetActive(!uiCanvas.activeSelf); }
    private void ToggleUI(bool newState) { uiCanvas.SetActive(newState); }
    private void TogglePlacePreview() { ToolPreview_PlaceTooltip.SetActive(!ToolPreview_PlaceTooltip.activeSelf); }
    private void TogglePlacePreview(bool newState) { ToolPreview_PlaceTooltip.SetActive(newState); }
    private void TogglePathfinding() {
        pathing_StartNode.SetActive(!pathing_StartNode.activeSelf);
        pathing_EndNode.SetActive(!pathing_EndNode.activeSelf);
    }
    private void TogglePathfinding(bool newState) {
        pathing_StartNode.SetActive(newState);
        pathing_EndNode.SetActive(newState);
    }
    private void ChangeMode(bool refresh = false) {
        switch (currentMode) { // A basic state machine, just switch to the next tool & change the tooltip
            case ToolMode.Pathfinding:
                if (refresh) {
                    ToolPreview_ToolName.GetComponent<Text>().text = "Pathfinding";
                    TogglePathfinding(true);
                    TogglePlacePreview(false);
                } else {
                    TogglePathfinding(false);
                    TogglePlacePreview(true);
                    ToolPreview_ToolName.GetComponent<Text>().text = "Terrain";
                    currentMode = ToolMode.Terrain;
                }
                break;
            case ToolMode.Terrain:
                if (refresh) {
                    ToolPreview_ToolName.GetComponent<Text>().text = "Terrain";
                } else {
                    ToolPreview_ToolName.GetComponent<Text>().text = "Objects";
                    currentMode = ToolMode.Objects;
                }
                break;
            case ToolMode.Objects:
                if (refresh) {
                    ToolPreview_ToolName.GetComponent<Text>().text = "Objects";
                } else {
                    TogglePathfinding(true);
                    TogglePlacePreview(false);
                    ToolPreview_ToolName.GetComponent<Text>().text = "Pathfinding";
                    currentMode = ToolMode.Pathfinding;
                }
                break;
        }
        selectedIndex = 0;
        ChangeSelectedPreview(selectedIndex);
    }
    private void ChangeMode(ToolMode newMode) { currentMode = newMode; ChangeMode(true); }
    private void UpdatePlacePreviewPosition() {
        Vector2 newOrigin = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 spritePos = Utils.V2toV2Int(newOrigin);
        if (currentMode == ToolMode.Objects) {
            spritePos = Utils.GetBottomLeftCornerOfObject(Utils.V2toV2Int(newOrigin), AssetManager.objectDefinitions[AssetManager.objectDefinitionsKeyList[selectedIndex]].Dimensions, placingOrientation);
        }
        ToolPreview_PlaceTooltip.transform.position = (Vector3)spritePos + Vector3.back;
    }
    private void UpdatePathVisual() {
        //First, get the path
        List<PathNode> path = Pathfinding.FindPath_AStar(
            MapManager.map.PathDataLayer, 
            Utils.WorldToGridPos(pathing_StartNode.transform.position, MapManager.mapSize),
            Utils.WorldToGridPos(pathing_EndNode.transform.position, MapManager.mapSize));

        if (path == null) return;
        //Draw lines between all the nodes that form the path
        //foreach (var n in path) {   
        //    //
        //}
    }
    private void ChangeSelectedPreview(int index = 0) {
        // TODO: ToolMode dependant stuff
        try {
            switch (currentMode) {
                case ToolMode.Pathfinding: // TODO:
                    previewImage.sprite = AssetManager.groundTypes["empty"].Sprite;
                    previewSelectedText.text = "None";

                    ToolPreview_PlaceTooltip.GetComponent<SpriteRenderer>().sprite = AssetManager.groundTypes["empty"].Sprite;
                    break;
                case ToolMode.Terrain:
                    previewImage.sprite = AssetManager.groundTypes[AssetManager.groundTypesKeyList[index]].Sprite;
                    previewSelectedText.text = AssetManager.groundTypes[AssetManager.groundTypesKeyList[index]].Name;

                    ToolPreview_PlaceTooltip.GetComponent<SpriteRenderer>().sprite = AssetManager.groundTypes[AssetManager.groundTypesKeyList[index]].Sprite;
                    break;
                case ToolMode.Objects:
                    previewImage.sprite = AssetManager.objectDefinitions[AssetManager.objectDefinitionsKeyList[index]].Sprite;
                    previewSelectedText.text = AssetManager.objectDefinitions[AssetManager.objectDefinitionsKeyList[index]].Name;

                    ToolPreview_PlaceTooltip.GetComponent<SpriteRenderer>().sprite = AssetManager.objectDefinitions[AssetManager.objectDefinitionsKeyList[index]].Sprite;
                    break;
            }
        } catch { throw; }
    }
    private void ChangeSelectedThing(int newIndex) {
        selectedIndex = newIndex;
        switch (currentMode) {
            case ToolMode.Pathfinding:
                ChangeSelectedPreview();
                break;
            case ToolMode.Terrain:
                if (selectedIndex >= AssetManager.groundTypesKeyList.Count) { selectedIndex = AssetManager.groundTypesKeyList.Count - 1; }
                ChangeSelectedPreview(selectedIndex);
                break;
            case ToolMode.Objects:
                if (selectedIndex >= AssetManager.objectDefinitionsKeyList.Count) { selectedIndex = AssetManager.objectDefinitionsKeyList.Count - 1; }
                ChangeSelectedPreview(selectedIndex);
                break;
        }
        //Debug.Log($"Final Index: {selectedIndex}"); //! Debugging
    }

    // Keyboard Input
    private void ManageKeyboardInputs() {
        // Hide UI (F4)
        if (Input.GetKeyDown(KeyCode.F4)) { ToggleUI(); }

        // Camera (Target) Moovement / Panning (WASD/Arrows)
        Vector2 cameraPanDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (cameraPanDirection != Vector2.zero) {
            // Move the camera target
            cameraTarget += cameraPanDirection * panningSpeed * Time.deltaTime * deltaTimeScaler; // Time.deltaTime is for framerate dependant stuff
            // Clamp the target, so it can't go outside of the map
            cameraTarget = Utils.DisplaceWorldPosInsideBounds(cameraTarget, MapManager.mapSize);

        }
        SmoothMoveCameraTowards(cameraTarget, 0.1f); // Move the camera towards the target

        // Camera Zooming (Mouse wheel)
        if (Input.mouseScrollDelta.y != 0) { // If you use the mouse wheel
            //Debug.Log("Why you spin me?"); //! Debugging
            if (Input.mouseScrollDelta.y > 0) { cameraZoom -= zoomModifier; }
            if (Input.mouseScrollDelta.y < 0) { cameraZoom += zoomModifier; }
            cameraZoom = Mathf.Clamp(cameraZoom, minZoom, maxZoom);
            // Clamp the zoom to the min and max zooms
            UpdateCameraZoom();
        }

        // Change ToolMode (Space)
        if (Input.GetKeyDown(KeyCode.Space)) { ChangeMode(); }

        // Rotate thing (Q & E)
        if (Input.GetKeyDown(KeyCode.Q)) { // Rotate Left
            switch (placingOrientation) {
                case Orientation.South:
                    placingOrientation = Orientation.East;
                    break;
                case Orientation.West:
                    placingOrientation = Orientation.South;
                    break;
                case Orientation.North:
                    placingOrientation = Orientation.West;
                    break;
                case Orientation.East:
                    placingOrientation = Orientation.North;
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.E)) { // Rotate Right
            switch (placingOrientation) {
                case Orientation.South:
                    placingOrientation = Orientation.West;
                    break;
                case Orientation.West:
                    placingOrientation = Orientation.North;
                    break;
                case Orientation.North:
                    placingOrientation = Orientation.East;
                    break;
                case Orientation.East:
                    placingOrientation = Orientation.South;
                    break;
            }
        }

        // Change Selected Thing (1-2-3-4-5-6-7-8-9-0)
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ChangeSelectedThing(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ChangeSelectedThing(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ChangeSelectedThing(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ChangeSelectedThing(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ChangeSelectedThing(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { ChangeSelectedThing(5); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { ChangeSelectedThing(6); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { ChangeSelectedThing(7); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { ChangeSelectedThing(8); }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { ChangeSelectedThing(9); }

        // Recalculate Path
        if (Input.GetKeyDown(KeyCode.P)) {
            UpdatePathVisual();
        }

        // Depending on the current ToolMode, do different things 
    }

    // Camera Smoothing
    private Vector2 refVelocity = Vector2.zero;
    private void SmoothMoveCameraTowards(Vector2 target, float timeToSmooth = 0.1f) {
        // Move towards the handle
        CameraHandle.transform.position = Vector2.SmoothDamp(CameraHandle.transform.position, target, ref refVelocity, timeToSmooth);
    }

    // Camera zooming
    private void UpdateCameraZoom() {
        gameCamera.orthographicSize = cameraZoom;
        panningSpeed = Mathf.Lerp(minPanningSpeed, maxPanningSpeed, Mathf.InverseLerp(minZoom, maxZoom, cameraZoom));
    }

    // Mouse Input
    private void ManageMouseInputs() {
        if (Input.GetMouseButtonDown(0)) {  // Left click
            // Get the mouse's world position
            Vector2 mousePos = gameCamera.ScreenToWorldPoint(Input.mousePosition);
            // Transform it to grid positions
            Vector2Int mouseGridPos = Utils.WorldToGridPos(mousePos, MapManager.mapSize);
            //Debug.Log($"Pos: {mousePos}, GridPos: {mouseGridPos}"); //! Debugging

            // Depending on the ToolMode, do different things (On Left Click)
            switch (currentMode) {
                case ToolMode.Pathfinding:
                    // TODO: On left click, place the start node
                    Vector2 newPos = Utils.DisplaceWorldPosInsideBounds(Utils.V2toV2Int(mousePos), MapManager.mapSize);
                    pathing_StartNode.transform.position = (Vector3)newPos + Vector3.back;
                    break;
                case ToolMode.Terrain:
                    // TODO:
                    try {
                        MapManager.map.GroundLayer.PlaceGroundTile(
                            mouseGridPos,
                            AssetManager.groundTypes[AssetManager.groundTypesKeyList[selectedIndex]]);
                    } catch { throw; }
                    break;
                case ToolMode.Objects:
                    try {
                        MapManager.map.ObjectLayer.PlaceObject(
                            AssetManager.objectDefinitions[AssetManager.objectDefinitionsKeyList[selectedIndex]],
                            mouseGridPos, placingOrientation);
                    } catch { throw; }
                    break;
            }
        }
        if (Input.GetMouseButtonDown(1)) { // Right click
            // Get the mouse's world position
            Vector2 mousePos = gameCamera.ScreenToWorldPoint(Input.mousePosition);
            // Transform it to grid positions
            Vector2Int mouseGridPos = Utils.WorldToGridPos(mousePos, MapManager.mapSize);
            //Debug.Log($"Pos: {mousePos}, GridPos: {mouseGridPos}"); //! Debugging

            // Depending on the ToolMode, do different things (On Right Click)
            switch (currentMode) {
                case ToolMode.Pathfinding:
                    // TODO: On right click, place the end node
                    Vector2 newPos = Utils.DisplaceWorldPosInsideBounds(Utils.V2toV2Int(mousePos), MapManager.mapSize);
                    pathing_EndNode.transform.position = (Vector3)newPos + Vector3.back;
                    break;
                case ToolMode.Terrain:
                    // TODO:
                    break;
                case ToolMode.Objects:
                    try {
                        MapManager.map.ObjectLayer.DestroyObject(mouseGridPos);
                    } catch { throw; }
                    break;
            }
        }
    }
}
