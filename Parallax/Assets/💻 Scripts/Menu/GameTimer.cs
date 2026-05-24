using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private NetworkVariable<double> _sceneStartTime = new();

    private void Start()
    {
        if (IsServer) {_sceneStartTime.Value = NetworkManager.ServerTime.Time;}
        InvokeRepeating(nameof(UpdateTimerText), 0f, 0.1f);
    }

    private void UpdateTimerText()
    {
        double elapsed = NetworkManager.ServerTime.Time - _sceneStartTime.Value;

        int hours = Mathf.FloorToInt((float)(elapsed / 3600));
        int minutes = Mathf.FloorToInt((float)(elapsed % 3600 / 60));
        int seconds = Mathf.FloorToInt((float)(elapsed % 60));
        int miliSeconds = Mathf.FloorToInt((float)(elapsed * 10f)) % 10;

        timerText.SetText("Time {0:00}:{1:00}:{2:00}.{3}",
            hours, minutes, seconds, miliSeconds
        );
    }
}