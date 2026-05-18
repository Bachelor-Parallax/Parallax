using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    [SerializeField] private Light torchLight;
    [SerializeField] private float minIntensity = 2f;
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField] private float flickerSpeed = 8f;

    private void Reset()
    {
        torchLight = GetComponent<Light>();
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        torchLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}