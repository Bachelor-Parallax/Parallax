using UnityEngine;

public interface IMovement
{
    void Move(Vector2 input, Vector3 forward, Vector3 right);
}