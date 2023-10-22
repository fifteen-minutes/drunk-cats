#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

[Serializable]
public class Bar
{
    [Serializable]
    public class Room
    {
        public int Id { get; internal set; }
        public Vector2Int PositionGridSpace;
        public RoomType RoomType;
    }
    
    public ReadOnlyCollection<Room?> Rooms => _rooms.AsReadOnly();

    [SerializeField] private List<Room?> _rooms = new();
    
    public static Bar? FromJson(string json)
    {
        Bar? bar = null;
        try
        {
            bar = JsonUtility.FromJson<Bar?>(json);
        }
        catch (ArgumentException e)
        {
            Debug.LogError(e);
        }
        return bar;
    }
    
    /// <summary>
    /// Problem of the deep future: RoomId is int32. Could be overfull someday.
    /// </summary>
    public void AddRoom(Vector2Int positionGridSpace, RoomType roomType)
    {
        Room room = new()
        {
            Id = _rooms.Count,
            PositionGridSpace = positionGridSpace,
            RoomType = roomType
        };
        _rooms.Add(room);
    }

    /// <summary>
    /// Returns false if roomId is less than 0 or equal or greater then Rooms.Count.
    /// Returns false if room with this index is already deleted.
    /// Returns true if room successfully deleted.
    /// </summary>
    public bool DeleteRoom(int roomId)
    {
        if (roomId < 0 || roomId >= _rooms.Count) return false;
        if (_rooms[roomId] == null) return false;
        _rooms[roomId] = null;
        return true;
    }

    public string ToJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }
}



public enum RoomType
{
    Default,
}
