using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    public GameObject DebugWhiteRoomPrefab;
    public GameObject DebugGrayRoomPrefab;

    private Transform _dragAndDropRoomPreview;

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1f);
        //print(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void BuildRoom(Vector2 gridPosition, GameObject roomPrefab)
    {
        GameObject newRoom = Instantiate(roomPrefab);
        newRoom.transform.position = gridPosition;
    }

    private void Update()
    {
        Vector2 pointerWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float x = Mathf.Floor(pointerWorldPosition.x) + 0.5f;
        float y = Mathf.Floor(pointerWorldPosition.y) + 0.5f;
        Vector2 pointerGridPosition = new(x, y);

        if (_dragAndDropRoomPreview)
        {
            _dragAndDropRoomPreview.position = pointerGridPosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            BuildRoom(pointerGridPosition, DebugWhiteRoomPrefab);
        }
    }

    private void Start()
    {
        _dragAndDropRoomPreview = Instantiate(DebugGrayRoomPrefab).transform;
    }
}
