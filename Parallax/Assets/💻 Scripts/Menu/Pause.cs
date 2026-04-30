using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private InputAction pauseAction;

    private void Awake()
    {
        pauseAction = new InputAction("Pause");
        pauseAction.AddBinding("<Keyboard>/escape");
        pauseAction.AddBinding("<Gamepad>/start");
        pauseAction.AddBinding("<Gamepad>/select"); // FallBack if controller dont have start
        pauseAction.performed += OnPause;
    }

    private void OnEnable() => pauseAction.Enable();
    private void OnDisable() => pauseAction.Disable();

    private void OnPause(InputAction.CallbackContext context)
    {
        bool isActive = !pauseMenu.activeSelf;
        pauseMenu.SetActive(isActive);
        Cursor.visible = isActive;
    }

    public void Resume()
    {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }
}