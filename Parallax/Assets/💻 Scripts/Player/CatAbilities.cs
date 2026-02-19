using UnityEngine;

[RequireComponent(typeof(BaseLocomotion))]
[RequireComponent(typeof(CharacterController))]
public class CatAbilities : MonoBehaviour, IJump
{
    [SerializeField] private float jumpImpulse = 8f;

    private BaseLocomotion locomotion;
    private CharacterController cc;

    void Awake()
    {
        locomotion = GetComponent<BaseLocomotion>();
        cc = GetComponent<CharacterController>();
    }

    public void Jump()
    {
        if (!cc.isGrounded) return;
        locomotion.AddVerticalImpulse(jumpImpulse);
    }
}
