using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public GameObject DebugWhiteRoomPrefab;
    public GameObject DebugGrayRoomPrefab;
    public float MaxMouseDeltaToAllowBuildRoomGridSpace = 0.05f;

    private Transform _dragAndDropRoomPreview;
    private Vector2? _touchStartGridPosition;


    public void BuildRoom(Vector2Int gridIntPosition, GameObject roomPrefab, Settings settings)
    {
        GameObject newRoom = Instantiate(roomPrefab);
        newRoom.transform.position = GridToWorldPosition(gridIntPosition + Vector2Int.one, settings);
    }

    private void Update()
    {
        Camera camera = Camera.main;
        if (camera == null) return;
        
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            Debug.LogError("SettingsManager game object with its script does not exist on the scene.");
            return;
        }
        Settings settings = settingsManager.Settings;
        
        Vector2 pointerWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pointerGridPosition = WorldToGridPosition(pointerWorldPosition, settings);
        Vector2Int pointerGridIntPosition = Vector2Int.FloorToInt(pointerGridPosition);
        
        bool inputBuildRoom = false;
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartGridPosition = pointerGridPosition;
        }
        else if (Input.GetMouseButtonUp(0) && _touchStartGridPosition.HasValue) 
        {
            Vector2 deltaGridSpace = pointerGridPosition - _touchStartGridPosition.Value;
            inputBuildRoom = deltaGridSpace.sqrMagnitude < MaxMouseDeltaToAllowBuildRoomGridSpace * MaxMouseDeltaToAllowBuildRoomGridSpace &&
                Vector2Int.FloorToInt(_touchStartGridPosition.Value) == pointerGridIntPosition;

            if (MaxMouseDeltaToAllowBuildRoomGridSpace < 1e-5)
            {
                Debug.LogWarning("MaxMouseDeltaToAllowBuildRoom is zero. Input for building won't work.");
            }
            
            _touchStartGridPosition = null;
        }
        
        if (_dragAndDropRoomPreview)
        {
            _dragAndDropRoomPreview.position =
                GridToWorldPosition(Vector2Int.FloorToInt(pointerGridPosition) + Vector2Int.one, settings);
        }

        if (inputBuildRoom)
        {
            BuildRoom(Vector2Int.FloorToInt(pointerGridPosition), DebugWhiteRoomPrefab, settings);
        }
    }

    private void Start()
    {
        _dragAndDropRoomPreview = Instantiate(DebugGrayRoomPrefab).transform;
    }

    public static Vector2 WorldToGridPosition(Vector3 worldPosition, Settings settings)
    {
        Vector2 gridXInWorldSpace = settings.GridSpaceXInWorldSpace;
        Vector2 gridYInWorldSpace = settings.GridSpaceYInWorldSpace;
        // Grid Space Matrix:
        // { gridXInWorldSpace.x, gridYInWorldSpace.x }
        // { gridXInWorldSpace.y, gridYInWorldSpace.y }
        // Getting inverse Matrix, because:
        // gridSpacePosition = inverseGridSpaceMatrix * worldPosition
        // I will code matrix as two 2d vectors: 'x' and 'y'.
        // So 'x' is the first column of matrix and 'y' is the second.
        float gridSpaceDeterminant = gridXInWorldSpace.x * gridYInWorldSpace.y - gridXInWorldSpace.y * gridYInWorldSpace.x;
        Vector2 adjugateX = new(gridYInWorldSpace.y, -gridXInWorldSpace.y);
        Vector2 adjugateY = new(-gridYInWorldSpace.x, gridXInWorldSpace.x);
        Vector2 inverseGridSpaceMatrixX = adjugateX / gridSpaceDeterminant;
        Vector2 inverseGridSpaceMatrixY = adjugateY / gridSpaceDeterminant;
        float xGridSpace = inverseGridSpaceMatrixX.x * worldPosition.x + inverseGridSpaceMatrixY.x * worldPosition.y;
        float yGridSpace = inverseGridSpaceMatrixX.y * worldPosition.x + inverseGridSpaceMatrixY.y * worldPosition.y;
        return new Vector2(xGridSpace, yGridSpace);
    }

    public static Vector2 GridToWorldPosition(Vector2 gridPosition, Settings settings)
    {
        Vector2 gridXInWorldSpace = settings.GridSpaceXInWorldSpace;
        Vector2 gridYInWorldSpace = settings.GridSpaceYInWorldSpace;
        // Grid Space Matrix:
        // { gridXInWorldSpace.x, gridYInWorldSpace.x }
        // { gridXInWorldSpace.y, gridYInWorldSpace.y }
        // Formula:
        // worldCoordinates = transformMatrix * gridCoordinates
        float x = gridXInWorldSpace.x * gridPosition.x + gridYInWorldSpace.x * gridPosition.y;
        float y = gridXInWorldSpace.y * gridPosition.x + gridYInWorldSpace.y * gridPosition.y;
        return new Vector2(x, y);
    }
}
