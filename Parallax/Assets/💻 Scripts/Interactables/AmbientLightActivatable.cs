using UnityEngine;

public class AmbientLightActivatable : MonoBehaviour, IActivatable
{
    [SerializeField] private Color ambientColor = Color.gray;
    [SerializeField] private float intensity = 0.2f;

    public void Activate()
    {
        RenderSettings.ambientMode =
            UnityEngine.Rendering.AmbientMode.Flat;

        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = intensity;
    }
}