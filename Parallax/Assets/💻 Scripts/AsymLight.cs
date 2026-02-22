using UnityEngine;

public class AsymLight : MonoBehaviour
{
    private Light _light;

    private void Awake()
    {
        _light = GetComponent<Light>();
        if (_light == null) Debug.LogError("Asymmetrical Light is missing a light component");
    }
}