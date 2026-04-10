using UnityEngine;

public interface IJump
{
    void Jump();
}

public interface ISprint
{
    void SetSprinting(bool sprinting);
}

public interface IBoxMover
{
    bool CanMoveBoxes { get; }
}

public interface IInteractor
{
    void Interact();
}