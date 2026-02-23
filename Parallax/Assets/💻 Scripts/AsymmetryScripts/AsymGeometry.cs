using UnityEngine;

public class AsymGeometry : BaseAsymProperty<MeshFilter>
{
    [SerializeField] private Mesh _meshA;
    [SerializeField] private Mesh _meshB;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.mesh = profile == PerspectiveProfile.A ? _meshA : _meshB;
    }
}