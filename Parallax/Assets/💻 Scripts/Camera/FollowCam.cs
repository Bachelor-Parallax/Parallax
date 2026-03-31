using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCam : MonoBehaviour
{
    [CanBeNull] public Transform target;

    public float distance = 4f;
    public float sensitivity = 200f;
    public float minY = -20f;
    public float maxY = 60f;

    private float currentX = 0f;
    private float currentY = 20f;

    public float Yaw => currentX;
    public float Pitch => currentY;

    
    //TODO:FIXME Temporary solution to make players active and fix position
    private void SetPlayerActive()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Player"))
            {
                obj.SetActive(true);
                obj.transform.position = (new Vector3(1f, 3f, 1f));
            }
        }
    }
    
    
    
    private void LateUpdate()
    {
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        SetPlayerActive(); //TODO:FIXME calls a temporary fix to make the players be at the right pos.
        
        if (!target)
        {
            Debug.LogError("No target assigned to follow camera!");
            return;
        }

        HandleMouseInput();
        UpdateCameraPosition();
    }

    void HandleMouseInput()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            
            currentX += delta.x * sensitivity * Time.deltaTime;
            currentY -= delta.y * sensitivity * Time.deltaTime;
            
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * new Vector3(0, 0, -distance);
        
        transform.position = target.position + direction;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

        public void SetTarget(Transform newTarget, bool snapToDefaultAngles = false)
    {
        target = newTarget;

        if (snapToDefaultAngles)
        {
            currentX = 0f;
            currentY = 20f;
        }
    }
}