using UnityEngine;

public class AsymVisibility : MonoBehaviour
{
    [SerializeField] private PerspectiveProfile _profile;

    private Renderer[] _renderers;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
    }

    public void ApplyPerspectiveProfile(PerspectiveProfile profile)
    {
        bool visible = _profile == profile;

        foreach (var rend in _renderers)
        {
            rend.enabled = visible;
        }
    }
}