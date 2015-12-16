using UnityEngine;

/// <summary>
/// An interface for moving entities.
/// </summary>
public interface IMovingEntity
{
    Rigidbody2D rigidBody2D
    {
        get;
        set;
    }

    Vector2 velocity
    {
        get;
        set;
    }

    void SetVelocity(Vector2 velocity);
}
