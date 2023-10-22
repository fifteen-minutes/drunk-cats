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
        newRoom.transform.position = Geometry.GridToWorldPosition(gridIntPosition + Vector2Int.one, settings);
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
        Vector2 pointerGridPosition = Geometry.WorldToGridPosition(pointerWorldPosition, settings);
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
                Geometry.GridToWorldPosition(Vector2Int.FloorToInt(pointerGridPosition) + Vector2Int.one, settings);
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
}
