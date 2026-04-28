using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject canvas; 
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.SetActive(!canvas.activeSelf);
        }
    }
}
