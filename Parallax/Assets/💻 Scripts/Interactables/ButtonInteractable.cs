using Unity.Netcode;
using UnityEngine;

public class ButtonInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private KeyInteractable requiredKey;
    [SerializeField] private AudioClip buttonSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(buttonSound);
        }
    }

    public void Interact(GameObject interactor)
    {
        var interactorRoleController = interactor.GetComponentInParent<RoleController>();

        if (interactorRoleController == null)
        {
            Debug.Log("Interactor has no RoleController.");
            return;
        }

        if (interactorRoleController.CurrentRole != CharacterRole.Human)
        {
            Debug.Log("Only humans can interact with the button.");
            return;
        }

        if (requiredKey != null && !requiredKey.keyCollected.Value)
        {
            Debug.Log("Button pressed, but key not collected!");
            return;
        }

        Debug.Log("Button interaction accepted.");
        PressButtonServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void PressButtonServerRpc()
    {
        foreach (var target in targets)
        {
            if (target == null) continue;

            if (target.TryGetComponent<IActivatable>(out var activatable))
            {
                activatable.Activate();
                Debug.Log($"Activated {target.name} from button press.");
            }
            else
            {
                Debug.LogWarning($"{target.name} does not implement IActivatable.");
            }
        }

        PlayButtonSound();
        Debug.Log("Button pressed!");
    }

    public string GetInteractText()
    {
        return "Press [E] to push the button";
    }
}