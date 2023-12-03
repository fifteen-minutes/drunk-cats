#nullable enable
using UnityEngine;

/// <summary>
/// Utilities class to work with coordinate systems.
/// Grid Space is 2-dimensional space. (0, 0) - center.
/// Each unit square represents one grid.
/// </summary>
public class Geometry : MonoBehaviour
{
    public static Vector2 WorldToGridPosition(Vector3 worldPosition, Settings? settings = null)
    {
        settings ??= SettingsManager.Instance.Settings;
        
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

    public static Vector2 GridToWorldPosition(Vector2 gridPosition, Settings? settings = null)
    {
        settings ??= SettingsManager.Instance.Settings;
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

    public static Vector2 ScreenToWorldPosition(Vector3 screenPosition, Camera? camera = null)
    {
        if (camera == null)
        {
            camera = CameraController.Instance.Camera;
        }
        return camera.ScreenToWorldPoint(screenPosition);
    }
    
    public static Vector2 WorldToScreenPosition(Vector3 worldPosition, Camera? camera = null)
    {
        if (camera == null)
        {
            camera = CameraController.Instance.Camera;
        }
        return camera.WorldToScreenPoint(worldPosition);
    }

    public static Vector2 ScreenToGridPosition(Vector3 screenPosition, Camera? camera = null, Settings? settings = null)
    {
        if (camera == null)
        {
            camera = CameraController.Instance.Camera;
        }
        return WorldToGridPosition(ScreenToWorldPosition(screenPosition, camera), settings);
    }

    public static Vector2 GridToScreenPosition(Vector2 gridPosition, Camera? camera = null, Settings? settings = null)
    {
        if (camera == null)
        {
            camera = CameraController.Instance.Camera;
        }
        return WorldToScreenPosition(GridToWorldPosition(gridPosition, settings), camera);
    }
}
