#nullable enable
using UnityEditor.UI;
using UnityEngine;

public enum DestinationType
{
    NotSpecified,
    ToEat,
    ToExit,
}

public struct Destination
{
    public DestinationType Type;
    public Vector2 Position;

    public Destination(DestinationType type, Vector2 position)
    {
        Type = type;
        Position = position;
    }
}

[RequireComponent(typeof(MovableNpc))]
public class VisitorNpc : MonoBehaviour
{
    public bool HasStartEating { get; private set; }
    public bool HasFinishedEating { get; private set; }
    
    [SerializeField] private float _eatingSeconds = 1.5f;
    
    private MovableNpc _movableNpc = default!;
    private Destination? _destination; 
    
    private void Awake()
    {
        _movableNpc = GetComponent<MovableNpc>();
    }
    private void Update()
    {
        if (!_destination.HasValue && !HasStartEating)
        {
            _destination = new Destination(DestinationType.ToEat, FindToEat());
        }
        if (HasFinishedEating)
        {
            _destination = new Destination(DestinationType.ToExit, FindExit());
        }
        if (_destination.HasValue)
        {
            _movableNpc.Destination = _destination.Value.Position;
        }

        if (_destination.HasValue && _movableNpc.HasReachedDestination())
        {
            if (_destination.Value.Type == DestinationType.ToEat)
            {
                // TODO: eating animation etc
                HasStartEating = true;
                HasFinishedEating = true;
            }
            else if (_destination.Value.Type == DestinationType.ToExit)
            {
                // TODO: dissolve animation
                Destroy(gameObject);
            }
        }
    }

    private Vector2 FindExit()
    {
        return new(0, -10);
    }

    private Vector2 FindToEat()
    {
        return new(0, 10);
    }
}
