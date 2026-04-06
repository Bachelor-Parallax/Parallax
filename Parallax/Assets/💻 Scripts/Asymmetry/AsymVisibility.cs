using UnityEngine;

public class AsymVisibility : BaseAsymProperty<Renderer>
{
    [SerializeField] private CharacterRole visibleForRole;

    public void ApplyRole(CharacterRole currentRole)
    {
        AsymComponent.enabled = currentRole == visibleForRole;
    }

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        // Old system no longer used
    }
}