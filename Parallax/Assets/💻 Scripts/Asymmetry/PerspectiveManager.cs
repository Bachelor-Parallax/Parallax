using UnityEngine;

public class PerspectiveManager : MonoBehaviour
{
    private BaseAsymProperty[] _asymProperties;

    private void Awake()
    {
        _asymProperties = FindObjectsByType<BaseAsymProperty>(FindObjectsSortMode.None);
    }

    public void ApplyPerspective(CharacterRole role)
    {
        // apply perspective settings to every asymmetrical object in the scene
        foreach (BaseAsymProperty asymProperty in _asymProperties)
        {
            asymProperty.ApplyPerspectiveProfile(role);
        }
    }
}