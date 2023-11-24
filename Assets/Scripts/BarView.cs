#nullable enable
using System;
using System.Linq;
using UnityEngine;


/// <summary>
/// View for model Bar.cs.
/// </summary>
public class BarView : MonoBehaviour
{
    public static BarView Instance { get; private set; }= default!;

    
    public float MaxMouseDeltaToAllowBuildRoomGridSpace = 0.05f;
    public Transform RoomsParent = default!;

    [SerializeField] private Room _roomPrefab = default!;
    
    private Vector2? _touchStartGridPosition;

    public Room? FindRoomById(int roomId)
    {
        return RoomsParent.GetComponentsInChildren<Room>().FirstOrDefault(room => room.Id == roomId);
    }
    
    /// Finds room game object that corresponds to Bar.Room in model.
    public Room FindRoom(Bar.Room room)
    {
        Room? roomGameObject = FindRoomById(room.Id);
        if (roomGameObject == null)
        {
            Debug.LogError("Unable to find room in BarView component. Internal error.");
        }
        return roomGameObject!;
    }
    
    private void RebuildBarOnScene(BarModel model, Settings? settings = null)
    {
        DeleteAllRoomsFromScene();
        foreach (Bar.Room room in model.Bar.Rooms)
        {
            BuildRoomOnScene(room, _roomPrefab, settings);
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

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Singleton error.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        BarModel.Instance.BarChanged += OnBarModelChanged;
        OnBarModelChanged(new BarChange(BarChangeType.Rebuild, null), BarModel.Instance);
    }

    private void OnDestroy()
    {
        BarModel.Instance.BarChanged -= OnBarModelChanged;
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
    
    private void BuildRoomOnScene(Bar.Room room, Room roomPrefab, Settings? settings = null)
    {
        Room newRoom = Instantiate(roomPrefab, RoomsParent, true);
        newRoom.Id = room.Id;
        newRoom.transform.position = Geometry.GridToWorldPosition(room.PositionGridSpace + Vector2Int.one, settings);
    }
}
