using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuContainer;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private List<GameObject> gameObjects;
    
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
        bool isActive = !pauseMenuContainer.activeSelf;
        pauseMenuContainer.SetActive(isActive);
        Cursor.visible = isActive;

        if (isActive) // Makes sure that the PauseMenu is the first menu showen each time
        {
            Debug.LogWarning("Pause - " + isActive);
            foreach (GameObject obj in gameObjects)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Pause - " + isActive);
            pauseMenu.gameObject.SetActive(true);
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        pauseMenuContainer.SetActive(false);
        
        foreach (GameObject obj in gameObjects)
        {
            obj.SetActive(false);
        }
    }

    public void Disconnect()
    {
        Cursor.visible = true;
        if (MultiplayerManager.Instance == null) return;
        
        //_ = MultiplayerManager.Instance.Disconnect();

        _ = LeaveGame();
    }
    
    private async Task LeaveGame()
    {
        if (!NetworkManager.Singleton.IsServer)
            await MultiplayerManager.Instance.Disconnect();
        else
            SceneLoader.Instance.LoadGameScene("PlayableLobby");
    }

    public void Retry()
    {
        if (SceneLoader.Instance == null) return;
        SceneLoader.Instance.ReloadCurrentScene();
        Cursor.visible = false;
    }
    
}