using UnityEngine;

[RequireComponent(typeof(BaseLocomotion))]
public class HumanAbilities : MonoBehaviour, ISprint
{
    [SerializeField] private float sprintMultiplier = 1.6f;

    private BaseLocomotion locomotion;

    void Awake()
    {
        locomotion = GetComponent<BaseLocomotion>();
    }

    public void SetSprinting(bool sprinting)
    {
        locomotion.SetSpeedMultiplier(sprinting ? sprintMultiplier : 1f);
    }
}
