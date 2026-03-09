using UnityEngine;

[RequireComponent(typeof(BaseController))]
public class HumanAbilities : MonoBehaviour, ISprint
{
    [SerializeField] private float sprintMultiplier = 1.6f;

    private BaseController movement;

    void Awake()
    {
        movement = GetComponent<BaseController>();
    }

    public void SetSprinting(bool sprinting)
    {
        movement.SetSpeedMultiplier(sprinting ? sprintMultiplier : 1f);
    }
}