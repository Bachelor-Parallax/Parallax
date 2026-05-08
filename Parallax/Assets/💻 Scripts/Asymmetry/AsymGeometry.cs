using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AsymGeometry : BaseAsymProperty<MeshFilter>
{
#if UNITY_EDITOR
    [InfoBox("Attached Mesh Filter component required!")]
#endif
    [SerializeField] private Mesh _humanMesh;
    [SerializeField] private Mesh _catMesh;

    public override void ApplyPerspectiveProfile(CharacterRole role)
    {
        AsymComponent.mesh = role switch
        {
            CharacterRole.Human => _humanMesh,
            CharacterRole.Cat => _catMesh,
            _ => throw new ArgumentException("Unexpected CharacterRole")
        };
    }
}