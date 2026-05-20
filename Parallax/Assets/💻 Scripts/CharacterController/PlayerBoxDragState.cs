using UnityEngine;

public class PlayerBoxDragState : MonoBehaviour
{
    public bool IsDraggingBox { get; private set; }

    public void SetDraggingBox(bool dragging)
    {
        IsDraggingBox = dragging;
    }
}