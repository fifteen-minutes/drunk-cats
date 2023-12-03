#nullable enable
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using YG;
using System;
using System.Linq;


[Serializable]
public class Bar
{
    [Serializable]
    public struct Room
    {
        public int Id;
        public Vector2Int PositionGridSpace;
        public RoomType RoomType;
    }

    public List<Room> Rooms = new();
    
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
    
    public string ToJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }

    public Bar DeepClone()
    {
        return FromJson(ToJson()) ?? new Bar();
    }
}

public enum RoomType
{
    Default,
}

public enum BarChangeType
{
    AddRoom,
    RemoveRoom,
    Rebuild
}

public struct BarChange
{
    public readonly BarChangeType Type;
    public readonly Bar.Room? ChangedRoom;

    public BarChange(BarChangeType type, Bar.Room? changedRoom)
    {
        Type = type;
        ChangedRoom = changedRoom;
    }
}

public class BarModel : MonoBehaviour
{
    public event Action<BarChange, BarModel>? BarChanged;

    public static BarModel Instance = default!;
    public Bar Bar = new();

    [ConsoleMethod("SaveProgress", "Saves game progress.")]
    public static void SaveProgress_ConsoleMethod()
    {
        Debug.Assert(Instance != null);
        if (Instance == null) return;
        Instance.SaveProgress();
    }
    
    [ConsoleMethod("LoadProgress", "Loads game progress.")]
    public static void LoadProgress_ConsoleMethod()
    {
        Debug.Assert(Instance != null);
        if (Instance == null) return;
        Instance.LoadProgress();
    }

    [ConsoleMethod("ResetProgress", "Resets game progress.")]
    public static void ResetProgress_ConsoleMethod()
    {
        Debug.Assert(Instance != null);
        if (Instance == null) return;
        Instance.ResetProgress();
    }
    
    public void SaveProgress()
    {
        Debug.Log("Saving progress:");
        Debug.Log($"data:\n{Bar.ToJson(true)}");
        YandexGame.savesData.bar = Bar.DeepClone();
        YandexGame.SaveProgress();
    }

    public void LoadProgress()
    {
        Debug.Assert(YandexGame.savesData != null);
        if (YandexGame.savesData == null) return;
        Debug.Log("Loading progress:");
        Debug.Log($"data:\n{YandexGame.savesData.bar.ToJson(true)}");
        Bar = YandexGame.savesData.bar.DeepClone();
        BarChanged?.Invoke(new BarChange(BarChangeType.Rebuild, null), this);
    }

    public void ResetProgress()
    {
        YandexGame.ResetSaveProgress();
        LoadProgress();
    }

    /// Sets standard bar with one room. Used when game starts.
    public void SetStartupBar()
    {
        Bar = new Bar();
        AddRoom(Vector2Int.zero, RoomType.Default);
    }
    
    /// Problem of the deep future: RoomId is int32. Could be overfull someday.
    public void AddRoom(Vector2Int positionGridSpace, RoomType roomType)
    {
        int roomId = Bar.Rooms.Count;
        Bar.Room room = new()
        {
            Id = roomId,
            PositionGridSpace = positionGridSpace,
            RoomType = roomType
        };
        Bar.Rooms.Add(room);
        BarChanged?.Invoke(new BarChange(BarChangeType.AddRoom, room), this);
    }

    /// Returns false if roomId is less than 0 or equal or greater then Rooms.Count.
    /// Returns false if room with this index is already deleted.
    /// Returns true if room successfully deleted.
    public bool DeleteRoom(int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= Bar.Rooms.Count) return false;
        Bar.Room removedRoom = Bar.Rooms[roomIndex];
        for (int i = roomIndex; i < Bar.Rooms.Count; i++)
        {
            Bar.Rooms[i] = Bar.Rooms[i + 1];
        }
        BarChanged?.Invoke(new BarChange(BarChangeType.RemoveRoom, removedRoom), this);
        return true;
    }

    public Bar.Room? FindRoomByPosition(Vector2Int gridPosition)
    {
        return Bar.Rooms.FirstOrDefault(room => room.PositionGridSpace == gridPosition);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
