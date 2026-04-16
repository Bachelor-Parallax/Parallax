using UnityEngine;

public class CatVisionRoot : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float hiddenIntensity = 0f;
    [SerializeField] private float visibleIntensity = 2f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;

    private float currentIntensity = 0f;
    private float targetIntensity = 0f;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        propertyBlock = new MaterialPropertyBlock();

        ApplyIntensity(hiddenIntensity);
    }

    private void Update()
    {
        if (Mathf.Approximately(currentIntensity, targetIntensity))
            return;

        currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, fadeSpeed * Time.deltaTime);
        ApplyIntensity(currentIntensity);
    }

    public void SetTargetVisible(bool visible)
    {
        targetIntensity = visible ? visibleIntensity : hiddenIntensity;
    }

    private void ApplyIntensity(float intensity)
    {
        bool shouldEnable = intensity > 0.001f;

        foreach (var rend in renderers)
        {
            if (rend == null)
                continue;

            rend.enabled = shouldEnable;

            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(EmissionColorId, Color.green * intensity);
            rend.SetPropertyBlock(propertyBlock);
        }
    }
}