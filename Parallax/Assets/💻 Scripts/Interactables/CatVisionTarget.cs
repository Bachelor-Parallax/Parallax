using UnityEngine;

public class CatVisionTarget : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color glowColor = Color.cyan;
    [SerializeField] private float hiddenIntensity = 0f;
    [SerializeField] private float visibleIntensity = 2f;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Visibility")]
    [SerializeField] private bool hideWhenInactive = true;

    private float currentIntensity;
    private float targetIntensity;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        SetVisible(false, true);
    }

    private void Update()
    {
        currentIntensity = Mathf.Lerp(
            currentIntensity,
            targetIntensity,
            fadeSpeed * Time.deltaTime
        );

        ApplyGlow();
    }

    public void SetVisible(bool visible, bool instant = false)
    {
        targetIntensity = visible ? visibleIntensity : hiddenIntensity;

        foreach (Renderer r in renderers)
        {
            if (r != null && hideWhenInactive)
                r.enabled = visible;
        }

        if (instant)
            currentIntensity = targetIntensity;

        ApplyGlow();
    }

    private void ApplyGlow()
    {
        if (renderers == null) return;

        foreach (Renderer r in renderers)
        {
            if (r == null) continue;

            foreach (Material mat in r.materials)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", glowColor * currentIntensity);
            }
        }
    }
}