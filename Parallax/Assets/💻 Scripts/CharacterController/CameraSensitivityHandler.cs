using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSensitivityHandler : NetworkBehaviour
{
    private CinemachineInputAxisController axisController;
    private SettingsManager settingsManager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        SceneManager.sceneLoaded += OnSceneLoaded;

        SetupSensitivity();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (settingsManager != null)
        {
            settingsManager.OnCamaraSensitivityChanged -= UpdateSensitivity;
        }
    }

    private void SetupSensitivity()
    {
        axisController = FindFirstObjectByType<CinemachineInputAxisController>();
        settingsManager = SettingsManager.Instance;

        if (axisController == null || settingsManager == null)
            return;

        settingsManager.OnCamaraSensitivityChanged += UpdateSensitivity;

        UpdateSensitivity(settingsManager.GetMouseSensitivity());
    }

    private void UpdateSensitivity(float value)
    {
        if (axisController == null)
            return;

        axisController.Controllers[0].Input.Gain = value;
        axisController.Controllers[1].Input.Gain = -value;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupSensitivity();
    }
}