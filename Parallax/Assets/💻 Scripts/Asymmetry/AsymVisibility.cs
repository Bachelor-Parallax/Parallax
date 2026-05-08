using UnityEngine;

[DisallowMultipleComponent]
public class AsymVisibility : BaseAsymProperty<Renderer>
{
    [SerializeField] private CharacterRole _activeRole;

    public override void ApplyPerspectiveProfile(CharacterRole role)
    {
        AsymComponent.enabled = _activeRole == role;
    }
}