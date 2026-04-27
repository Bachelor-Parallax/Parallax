using Unity.Netcode;
using UnityEngine;

public class LightActivatable : NetworkBehaviour, IActivatable
{
    [SerializeField] private Light[] lights;
    [SerializeField] private bool startOn = false;

    private NetworkVariable<bool> isOn = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        if (lights == null || lights.Length == 0)
            lights = GetComponentsInChildren<Light>(true);
    }

    public override void OnNetworkSpawn()
    {
        isOn.OnValueChanged += OnLightStateChanged;

        if (IsServer)
            isOn.Value = startOn;

        ApplyLightState(isOn.Value);
    }

    public override void OnNetworkDespawn()
    {
        isOn.OnValueChanged -= OnLightStateChanged;
    }

    public void Activate()
    {
        if (!IsServer) return;

        isOn.Value = true;
    }

    private void OnLightStateChanged(bool oldValue, bool newValue)
    {
        ApplyLightState(newValue);
    }

    private void ApplyLightState(bool value)
    {
        foreach (Light light in lights)
        {
            if (light != null)
                light.enabled = value;
        }
    }
}