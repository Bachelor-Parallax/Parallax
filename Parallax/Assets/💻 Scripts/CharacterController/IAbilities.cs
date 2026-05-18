using UnityEngine;

public interface IJump
{
    void RequestJump();
    void ReleaseJump();
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
    void Interact(IInteractable target);
}