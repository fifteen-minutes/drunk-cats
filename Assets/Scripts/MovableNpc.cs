using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class MovableNpc : MonoBehaviour
{
    public Vector2? Destination;
    public float Velocity = 1f;

    public bool HasReachedDestination(float delta = 0.1f)
    {
        return Destination.HasValue && (transform.position - (Vector3)Destination.Value).sqrMagnitude <= delta * delta;
    }
    
    private void Update()
    {
        if (Destination.HasValue)
        {
            Vector3 direction = (Destination.Value - (Vector2)transform.position).normalized;
            transform.position += Velocity * Time.deltaTime * direction;
        }
    }
}
