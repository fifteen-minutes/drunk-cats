#nullable enable
using System;
using UnityEngine;

/// <summary>
/// Attach to Main Camera game object.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public event Action<Vector2>? ClickedOnGridCoordinates;

    
    /// 0 - left mouse button, 1 - right mouse button.
    public int MouseButtonToPan = 0;
    /// 0 - left mouse button, 1 - right mouse button.
    public int MouseButtonToSelect = 0;
    public float PanSensitivity = 1f;
    public float ZoomSensitivity = 1f;
    public float MinPanWorldSpace = 0.3f;
    public float InterpolationMultiplier = 10f;
    public GameObject? ObservedGameObject;
    public Camera Camera;

    [SerializeField] private float _minCameraOrthographicSize = 1;
    [SerializeField] private float _maxCameraOrthographicSize = 10;
    [SerializeField] private float _maxMouseDeltaGridSpaceForClick = 0.05f;

    private Vector2? _cameraStartPosition;
    private Vector2? _startClickPositionScreenSpace;
    private Vector2? _cameraDesiredPosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Singleton error.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Camera = GetComponent<Camera>();
        if (Camera == null)
        {
            Debug.LogError("This script must be attached to Camera gameobject. No camera found on this gameobject.");
            return;
        }
    }

    private void Update()
    {
        if (ObservedGameObject != null)
        {
            _cameraDesiredPosition = (Vector2)ObservedGameObject.transform.position;
        }

        Vector2? clicked = null;
        // Handle pan;
        if (Input.GetMouseButtonDown(MouseButtonToPan))
        {
            _startClickPositionScreenSpace = Input.mousePosition;
            _cameraStartPosition = transform.position;
        }
        else if (Input.GetMouseButton(MouseButtonToPan) &&
                 _startClickPositionScreenSpace.HasValue &&
                 _cameraStartPosition.HasValue)
        {
            Vector2 deltaWorldPosition = 
                Geometry.ScreenToWorldPosition(Input.mousePosition) -
                Geometry.ScreenToWorldPosition(_startClickPositionScreenSpace.Value);
            if (deltaWorldPosition.sqrMagnitude < MinPanWorldSpace * MinPanWorldSpace)
            {
                deltaWorldPosition = Vector2.zero;
            }

            _cameraDesiredPosition = _cameraStartPosition.Value + (-deltaWorldPosition * PanSensitivity);
        }
        else if (Input.GetMouseButtonUp(MouseButtonToPan) && _startClickPositionScreenSpace.HasValue)
        {
            Vector2 deltaGridSpace = 
                Geometry.ScreenToGridPosition(Input.mousePosition) -
                Geometry.ScreenToGridPosition(_startClickPositionScreenSpace.Value);
            Vector2 clickStartGridPosition = Geometry.ScreenToGridPosition(_startClickPositionScreenSpace.Value);
            Vector2 currentGridPosition = Geometry.ScreenToGridPosition(Input.mousePosition);
            if (deltaGridSpace.sqrMagnitude < _maxMouseDeltaGridSpaceForClick * _maxMouseDeltaGridSpaceForClick &&
                Vector2Int.FloorToInt(clickStartGridPosition) == Vector2Int.FloorToInt(currentGridPosition))
            {
                clicked = clickStartGridPosition;
            }

            if (_maxMouseDeltaGridSpaceForClick < 1e-5)
            {
                Debug.LogWarning("MaxMouseDeltaToAllowBuildRoom is zero. Input for building won't work.");
            }
            
            _startClickPositionScreenSpace = null;
            _cameraStartPosition = null;
        }
        
        if (clicked.HasValue)
        {
            ClickedOnGridCoordinates?.Invoke(clicked.Value);
        }

        
        // Handle zoom.
        float zoomDelta = -Input.mouseScrollDelta.y * ZoomSensitivity;
        Camera.orthographicSize = Mathf.Clamp(
            Camera.orthographicSize + zoomDelta,
                _minCameraOrthographicSize,
                _maxCameraOrthographicSize);

        // Update position.
        if (_cameraDesiredPosition.HasValue)
        {
            if (InterpolationMultiplier < 1e-5)
            {
                Debug.LogWarning("InterpolationMultiplier is zero. Camera pan won't work.");
            } 
            Vector3 newPosition = Vector2.Lerp(transform.position, _cameraDesiredPosition.Value, Time.deltaTime * InterpolationMultiplier);
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }
}
