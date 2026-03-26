using UnityEngine;

public class PerspectiveManager : MonoBehaviour, IPerspectiveManager
{
    private BaseAsymProperty[] _asymProperties;

    private void Awake()
    {
        _asymProperties = FindObjectsByType<BaseAsymProperty>(FindObjectsSortMode.None);
    }

    public void ApplyPerspective(PerspectiveProfile profile)
    {
        foreach (BaseAsymProperty asymProperty in _asymProperties)
        {
            asymProperty.ApplyPerspectiveProfile(profile);
        }
    }
}