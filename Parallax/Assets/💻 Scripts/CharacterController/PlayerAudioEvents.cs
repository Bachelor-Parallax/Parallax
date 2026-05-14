using System;
using UnityEngine;

public class PlayerAudioEvents : MonoBehaviour
{
    public event Action OnJump;
    public event Action OnCharacterSound;
    public event Action OnFootstep;

    public void Jump() => OnJump?.Invoke();
    public void CharacterSound() => OnCharacterSound?.Invoke();
    public void Footstep() => OnFootstep?.Invoke();
}