using UnityEngine;

[DisallowMultipleComponent]
public class AsymVisibility : BaseAsymProperty<Renderer>
{
    [InfoBox("Attached Renderer component required!\nRenderer will be enabled only for the selected role")]
    [SerializeField] private CharacterRole _activeRole;

    public override void ApplyPerspectiveProfile(CharacterRole role)
    {
        AsymComponent.enabled = _activeRole == role;
    }
}