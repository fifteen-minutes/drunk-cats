#nullable enable
using UnityEngine;


/// <summary>
/// View for model Bar.cs.
/// </summary>
public class BarView : MonoBehaviour
{
    public GameObject DebugWhiteRoomPrefab = default!;
    public GameObject DebugGrayRoomPrefab = default!;
    public float MaxMouseDeltaToAllowBuildRoomGridSpace = 0.05f;
    public Transform RoomsParent = default!;

    private Transform? _dragAndDropRoomPreview;
    private Vector2? _touchStartGridPosition;

    private void RebuildBarOnScene(BarModel model, Settings settings)
    {
        DeleteAllRoomsFromScene();
        foreach (Bar.Room room in model.Bar.Rooms)
        {
            BuildRoomOnScene(room, settings);
        }
    }

    private void DeleteAllRoomsFromScene()
    {
        if (RoomsParent == null)
        {
            Debug.LogError(RoomsParent);
            return;
        }
        foreach (Transform room in RoomsParent)
        {
            Destroy(room.gameObject);
        }
    }

    private void Update()
    {
        Camera? mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError(mainCamera);
            return;
        }
        
        SettingsManager settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            Debug.LogError("SettingsManager game object with its script does not exist on the scene.");
            return;
        }
        Settings settings = settingsManager.Settings;
        
        Vector2 pointerWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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
        
        if (_dragAndDropRoomPreview != null)
        {
            _dragAndDropRoomPreview.position =
                Geometry.GridToWorldPosition(Vector2Int.FloorToInt(pointerGridPosition) + Vector2Int.one, settings);
        }

        if (inputBuildRoom)
        {
            BarModel.Instance!.AddRoom(Vector2Int.FloorToInt(pointerGridPosition), RoomType.Default);
        }
    }

    private void Start()
    {
        _dragAndDropRoomPreview = Instantiate(DebugGrayRoomPrefab).transform;
        if (BarModel.Instance != null)
        {
            BarModel.Instance.BarChanged += OnBarModelChanged;
        }
    }

    private void OnDestroy()
    {
        if (BarModel.Instance != null)
        {
            BarModel.Instance.BarChanged -= OnBarModelChanged;
        }
    }

    private void OnBarModelChanged(BarChange barChange, BarModel barModel)
    {
        Settings settings = SettingsManager.Instance.Settings;
        if (barChange.Type == BarChangeType.Rebuild)
        {
            RebuildBarOnScene(barModel, settings);
        }
        // TODO: handle all BarChangedTypes properly.
        RebuildBarOnScene(barModel, settings);

    }
    
    private void BuildRoomOnScene(Vector2Int gridIntPosition, GameObject roomPrefab, Settings settings)
    {
        GameObject newRoom = Instantiate(roomPrefab, RoomsParent, true);
        newRoom.transform.position = Geometry.GridToWorldPosition(gridIntPosition + Vector2Int.one, settings);
    }
    
    private void BuildRoomOnScene(Bar.Room room, Settings settings)
    {
        BuildRoomOnScene(room.PositionGridSpace, DebugWhiteRoomPrefab, settings);
    }
}
