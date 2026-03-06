using UnityEngine;

public class PerspectiveManager : MonoBehaviour
{
    private BaseAsymProperty[] _asymProperties;

    private void Awake()
    {
        _asymProperties = FindObjectsByType<BaseAsymProperty>(FindObjectsSortMode.None);
    }

    /// <summary>
    /// Applies all perspective settings for a profile
    /// </summary>
    public void ApplyPerspective(PerspectiveProfile profile)
    {
        foreach (BaseAsymProperty asymProperty in _asymProperties)
        {
            asymProperty.ApplyPerspectiveProfile(profile);
        }
    }
}