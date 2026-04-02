using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PerspectiveManager : MonoBehaviour, IPerspectiveManager
{
    private BaseAsymProperty[] _asymProperties;

    private NetworkManager _networkManager;

    private void Awake()
    {
        _asymProperties = FindObjectsByType<BaseAsymProperty>(FindObjectsSortMode.None);
        _networkManager = FindFirstObjectByType<NetworkManager>();

        if (_networkManager == null)
        {
            Debug.LogError("No INetworkManager found in scene.");
        }
    }

    public void ApplyPerspective()
    {
        // find target perspective
        //GameObject localPlayer = _networkManager.GetLocalPlayerGameObject();
        if (_networkManager.LocalClient?.PlayerObject == null)
        {
            Debug.LogWarning("Local player not spawned yet.");
            return;
        }
        
        GameObject localPlayer = _networkManager.LocalClient.PlayerObject.gameObject;

        PerspectiveProfile profile = localPlayer.GetComponent<RoleController>().role.Value switch
        {
            CharacterRole.Cat => PerspectiveProfile.Cat,
            CharacterRole.Human => PerspectiveProfile.Human,
            _ => throw new NotSupportedException(
                $"Role not supported: {localPlayer.GetComponent<RoleController>().role}")
        };

        // apply perspective settings to every asymmetrical object in the scene
        foreach (BaseAsymProperty asymProperty in _asymProperties)
        {
            asymProperty.ApplyPerspectiveProfile(profile);
        }
    }
}