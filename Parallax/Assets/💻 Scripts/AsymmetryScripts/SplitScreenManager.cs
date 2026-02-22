using UnityEngine;

/// <summary>
/// A debugging and test tool that simulates split screen play,
/// by hooking into existing architecture and modding the game
/// </summary>

public class SplitScreenManager : MonoBehaviour
{
    private AsymmetricalLight[] _asymmetricalLights;
    private AsymmetricalMeshRenderer[] _asymmetricalMeshRenderers;

    private void Awake()
    {
        _asymmetricalLights = FindObjectsByType<AsymmetricalLight>(FindObjectsSortMode.None);
        _asymmetricalMeshRenderers = FindObjectsByType<AsymmetricalMeshRenderer>(FindObjectsSortMode.None);
    }

    public void ApplyPerspective(PerspectiveProfile profile)
    {
        foreach (AsymmetricalLight asymmetricalLight in _asymmetricalLights)
        {
            asymmetricalLight.ApplyPerspectiveProfile(profile);
        }

        foreach (AsymmetricalMeshRenderer asymmetricalMeshRenderer in _asymmetricalMeshRenderers)
        {
            asymmetricalMeshRenderer.ApplyPerspectiveProfile(profile);
        }
    }
}