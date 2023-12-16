#nullable enable
using System;
using IngameDebugConsole;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    public static NpcSpawner Instance { get; private set; } = default!;

    [SerializeField] private VisitorNpc _visitorNpcPrefab = default!;

    private Transform? _npcParent;

    [ConsoleMethod("SpawnNpc", "Spawn a npc at specified position. For example: SpawnNpc [10 0]", "position")]
    public static void SpawnNpc_ConsoleMethod(Vector2 position)
    {
        Instance.SpawnNpc(position);
    }
    
    public void SpawnNpc(Vector2 position)
    {
        Instantiate(_visitorNpcPrefab, position, Quaternion.identity, parent: transform);
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
}
