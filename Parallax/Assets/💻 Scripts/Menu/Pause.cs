using System;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameObject.SetActive(!_gameObject.activeSelf);
        }
    }
}
