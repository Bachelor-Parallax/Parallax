using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuOnlyPreSelectItem : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelected;
    private void Start() // Just used for the main menu (first menu, as that does not get enabled)
    {
        EventSystem.current.SetSelectedGameObject(defaultSelected.gameObject);
    }
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelected.gameObject);
    }
}
