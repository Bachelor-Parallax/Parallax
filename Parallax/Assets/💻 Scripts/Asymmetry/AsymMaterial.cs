using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AsymMaterial : BaseAsymProperty<Renderer>
{
    [InfoBox("Attached Renderer component required!")]
    [SerializeField] private Material _humanMaterial;
    [SerializeField] private Material _catMaterial;

    public override void ApplyPerspectiveProfile(CharacterRole role)
    {
        AsymComponent.material = role switch
        {
            CharacterRole.Human => _humanMaterial,
            CharacterRole.Cat => _catMaterial,
            _ => throw new ArgumentException("Unexpected CharacterRole")
        };
    }
}