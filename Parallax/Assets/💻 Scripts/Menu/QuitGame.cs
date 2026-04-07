using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit() {
    #if UNITY_EDITOR
        // Stop play mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // Quit the application in a build
        Application.Quit();
    #endif
    }
}

