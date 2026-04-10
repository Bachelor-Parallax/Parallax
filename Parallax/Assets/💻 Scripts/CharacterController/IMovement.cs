using UnityEngine;

public interface IMovement
{
    void Move(Vector2 input);
    /// <summary>
    /// Rotates the character based on look input. This is separate from Move to allow for different rotation behavior when carrying boxes or other objects.
    /// </summary>
    // void Rotate(Vector2 lookInput);
}