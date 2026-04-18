using System;
using UnityEngine;

public class AsymGeometry : BaseAsymProperty<MeshFilter>
{
    [InfoBox("Attached Mesh Filter component required!\nMesh updated at runtime to make object appear visually different")]
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