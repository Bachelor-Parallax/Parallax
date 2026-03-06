using System;
using UnityEngine;

public class AsymGeometry : BaseAsymProperty<MeshFilter>
{
    [SerializeField] private Mesh _humanMesh;
    [SerializeField] private Mesh _catMesh;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.mesh = profile switch
        {
            PerspectiveProfile.Human => _humanMesh,
            PerspectiveProfile.Cat => _catMesh,
            _ => throw new ArgumentException("Unexpected perspective profile")
        };
    }
}