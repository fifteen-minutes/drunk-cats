#nullable enable
using System;
using UnityEngine;

public enum GameState
{
    Observe,
    Edit,
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; } = default!;

    public GameState GameState { get; private set; }
    
    [SerializeField] private GameObject _grid = default!;

    public bool IsGridVisible => _grid.activeInHierarchy;
    
    public void ShowGrid(bool value)
    {
        _grid.SetActive(value);
    }

    public void ChangeGameState(GameState gameState)
    {
        if (gameState == GameState.Observe)
        {
            ShowGrid(false);
        }
        else if (gameState == GameState.Edit)
        {
            ShowGrid(true);
        }
        else
        {
            Debug.LogError("Unhandled state.");
        }
        GameState = gameState;
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
        BarModel.Instance.SetStartupBar();
        CameraController.Instance.ClickedOnGridCoordinates += OnClickedOnGridCoordinates;
    }

    private void OnDestroy()
    {
        CameraController.Instance.ClickedOnGridCoordinates -= OnClickedOnGridCoordinates;
    }

    private void OnClickedOnGridCoordinates(Vector2 gridPosition)
    {
        Bar.Room? selectedRoom = BarModel.Instance.FindRoomByPosition(Vector2Int.FloorToInt(gridPosition));
        if (!selectedRoom.HasValue) return;
        if (GameState == GameState.Observe)
        {
            ChangeGameState(GameState.Edit);
            Room selectedRoomGameObject = BarView.Instance.FindRoom(selectedRoom.Value);
            CameraController.Instance.ObservedGameObject = selectedRoomGameObject.gameObject;
        }
    }
}
