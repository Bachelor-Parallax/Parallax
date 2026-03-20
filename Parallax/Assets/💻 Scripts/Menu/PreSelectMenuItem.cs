using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreSelectMenuItem : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelected;
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelected.gameObject);
    }
}
