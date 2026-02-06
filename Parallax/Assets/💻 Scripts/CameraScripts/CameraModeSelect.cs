using System.Linq;
using UnityEngine;

public class CameraModeSelect : MonoBehaviour
{
    private Camera _mainCam, _secondaryCam;
    private bool _splitScreenMode = true;

    private void Awake()
    {
        // find the camera component on this game object
        _mainCam = GetComponent<Camera>();
        if (_mainCam == null) Debug.LogError("Missing camera for main player");

        // find the other camera in the scene
        _secondaryCam =
            FindObjectsByType<Camera>(FindObjectsSortMode.None)
            .FirstOrDefault(c => c != _mainCam);
        if (_secondaryCam == null) Debug.LogError("Missing camera for secondary player");
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.F1)) return;

        if (_splitScreenMode)
        {
            _mainCam.rect = new Rect(0f, 0f, 1f, 1f);
            _secondaryCam.enabled = false;
            _splitScreenMode = false;
        }
        else
        {
            _mainCam.rect = new Rect(0f, 0f, 0.5f, 1f);
            _secondaryCam.enabled = true;
            _splitScreenMode = true;
        }
    }
}