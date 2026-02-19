using UnityEngine;

public interface ILocomotion
{
    void Move(Vector2 move);
    void Rotate(Vector2 look);
}

public interface IJump
{
    void Jump();
}

public interface ISprint
{
    void SetSprinting(bool sprinting);
}
