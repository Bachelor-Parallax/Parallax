using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCam : MonoBehaviour
{
    [CanBeNull] public Transform target;

    public float distance = 4f;
    public float sensitivity = 10f;
    public float minY = -20f;
    public float maxY = 60f;

    [SerializeField] private float zoomSmoothSpeed = 6f;

    private float currentX = 0f;
    private float currentY = 20f;

    private float currentDistance;
    private float targetDistance;

    public float Yaw => currentX;
    public float Pitch => currentY;

    private void Awake()
    {
        currentDistance = distance;
        targetDistance = distance;
    }

    private void LateUpdate()
    {
        if (!target)
        {
            Debug.LogError("No target assigned to follow camera!");
            return;
        }

        HandleMouseInput();

        currentDistance = Mathf.Lerp(
            currentDistance,
            targetDistance,
            zoomSmoothSpeed * Time.deltaTime
        );

        UpdateCameraPosition();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            currentX += delta.x * sensitivity * Time.deltaTime;
            currentY -= delta.y * sensitivity * Time.deltaTime;

            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * new Vector3(0, 0, -currentDistance);

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

    public void SetTargetDistance(float newDistance)
    {
        targetDistance = newDistance;
    }

    public float GetBaseDistance()
    {
        return distance;
    }
}