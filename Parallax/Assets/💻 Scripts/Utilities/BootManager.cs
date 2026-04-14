using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    public float bootDuration = 4f;
    
    void Start()
    {
        Invoke(nameof(LoadMenu), bootDuration);
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
