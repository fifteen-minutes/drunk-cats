using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Attach to Main Camera game object.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// 0 - left mouse button, 1 - right mouse button.
    /// </summary>
    public int MouseButtonToPan = 0;
    public float PanSensitivity = 1f;
    public float ZoomSensitivity = 1f;
    public float MinPanWorldSpace = 0.3f;
    public float InterpolationMultiplier = 10f;
    public Camera Camera;

    [SerializeField] private float _minCameraOrthographicSize = 1;
    [SerializeField] private float _maxCameraOrthographicSize = 10;

    private Vector2? _cameraStartPosition;
    private Vector2? _startPanPositionScreenSpace;
    private Vector2? _cameraDesiredPosition;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        if (Camera == null)
        {
            Debug.LogError("This script must be attached to Camera gameobject. No camera found on this gameobject.");
            return;
        }
    }

    private void Update()
    {
        // Handle input.
        if (Input.GetMouseButtonDown(MouseButtonToPan))
        {
            _startPanPositionScreenSpace = Input.mousePosition;
            _cameraStartPosition = transform.position;
        }
        else if (Input.GetMouseButton(MouseButtonToPan) &&
            _startPanPositionScreenSpace.HasValue &&
            _cameraStartPosition.HasValue)
        {
            Vector2 delta = Camera.ScreenToWorldPoint(Input.mousePosition) - Camera.ScreenToWorldPoint(_startPanPositionScreenSpace.Value);
            if (delta.sqrMagnitude < MinPanWorldSpace * MinPanWorldSpace)
            {
                delta = Vector2.zero;
            }
            Vector2 newPosition = _cameraStartPosition.Value + (-delta * PanSensitivity);
            _cameraDesiredPosition = newPosition;
        }
        else if (Input.GetMouseButtonUp(MouseButtonToPan))
        {
            _startPanPositionScreenSpace = null;
            _cameraStartPosition = null;
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
