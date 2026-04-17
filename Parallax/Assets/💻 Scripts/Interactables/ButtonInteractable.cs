using Unity.Netcode;
using UnityEngine;

public class ButtonInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private MonoBehaviour[] targets;
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
        var interactorRoleController = interactor.GetComponent<RoleController>();

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

        if (requiredKey == null)
        {
            Debug.Log("Button pressed, but requiredKey is NULL");
            return;
        }

        Debug.Log($"requiredKey exists, keyCollected = {requiredKey.keyCollected.Value}");

        if (!requiredKey.keyCollected.Value)
        {
            Debug.Log("Button pressed, but key not collected!");
            return;
        }

        PressButtonServerRpc();
        PlayButtonSound();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void PressButtonServerRpc()
    {
        foreach (var target in targets)
        {
            if (target is IActivatable activatable)
            {
                activatable.Activate();
            }
        }

        Debug.Log("Button pressed!");
    }

    public string GetInteractText()
    {
        return "Press button";
    }
}