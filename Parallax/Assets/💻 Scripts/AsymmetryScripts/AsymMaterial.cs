using UnityEngine;

public class AsymMaterial : AsymRenderer
{
    [SerializeField] private Material materialA;
    [SerializeField] private Material MaterialB;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.material = profile == PerspectiveProfile.A ? materialA : MaterialB;
    }
}