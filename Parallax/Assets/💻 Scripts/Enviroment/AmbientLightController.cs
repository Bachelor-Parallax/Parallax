using UnityEngine;

public class AmbientLightController : MonoBehaviour
{
    [SerializeField] private Color ambientColor = Color.white;
    [SerializeField] private float intensity = 1f;

    private void Start()
    {
        ApplyLighting();
    }

    public void ApplyLighting()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = intensity;
    }

    public void SetAmbient(Color color, float newIntensity)
    {
        ambientColor = color;
        intensity = newIntensity;

        ApplyLighting();
    }
}