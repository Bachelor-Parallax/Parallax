using UnityEngine;

[RequireComponent(typeof(BaseController))]
[RequireComponent(typeof(CharacterController))]
public class CatAbilities : MonoBehaviour, IJump
{
    [SerializeField] private float jumpImpulse = 8f;

    private BaseController controller;
    private CharacterController cc;

    void Awake()
    {
        controller = GetComponent<BaseController>();
        cc = GetComponent<CharacterController>();
    }

    public void Jump()
    {
        if (!cc.isGrounded) return;
        controller.AddVerticalImpulse(jumpImpulse);
    }
}
