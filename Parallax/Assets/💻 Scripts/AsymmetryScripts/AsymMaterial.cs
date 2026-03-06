using System;
using UnityEngine;

public class AsymMaterial : BaseAsymProperty<Renderer>
{
    [SerializeField] private Material _humanMaterial;
    [SerializeField] private Material _catMaterial;

    public override void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        AsymComponent.material = profile switch
        {
            PerspectiveProfile.Human => _humanMaterial,
            PerspectiveProfile.Cat => _catMaterial,
            _ => throw new ArgumentException("Unexpected perspective profile")
        };
    }
}