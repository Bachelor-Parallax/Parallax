using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AsymLight : BaseAsymProperty<Light>
{
    [InfoBox("Attached light component required!\nThe attached light component will be updated at runtime to match configured settings")]
    [Header("Cat Settings")]
    [SerializeField] private Color _lightColorCat;
    [SerializeField] private float _intensityCat;

    [Header("Human Settings")]
    [SerializeField] private Color _lightColorHuman;
    [SerializeField] private float _intensityHuman;

    public override void ApplyPerspectiveProfile(CharacterRole role)
    {
        if (role == CharacterRole.Human)
        {
            AsymComponent.color = _lightColorHuman;
            AsymComponent.intensity = _intensityHuman;
        }
        else if (role == CharacterRole.Cat)
        {
            AsymComponent.color = _lightColorCat;
            AsymComponent.intensity = _intensityCat;
        }
        else throw new ArgumentException("Unexpected CharacterRole");
    }
}