using System;
using UnityEngine;

public class PlayerAudioEvents : MonoBehaviour
{
    public event Action OnJump;
    public event Action OnDialogue;
    public event Action OnFootstep;

    public void Jump() => OnJump?.Invoke();
    public void Dialogue() => OnDialogue?.Invoke();
    public void Footstep() => OnFootstep?.Invoke();
}