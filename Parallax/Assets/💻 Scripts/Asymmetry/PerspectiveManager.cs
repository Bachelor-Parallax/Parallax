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
        // _networkManager = UnityExtensions.FindObjectsAssignableTo<INetworkManager>(FindObjectsSortMode.None)
        //     .FirstOrDefault();
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
        
        PerspectiveProfile profile = localPlayer.tag switch
        {
            GameConstants.CAT_TAG => PerspectiveProfile.Cat,
            GameConstants.HUMAN_TAG => PerspectiveProfile.Human,
            _ => throw new NotSupportedException($"Tag not supported: {localPlayer.tag}")
        };

        // apply perspective settings to every asymmetrical object in the scene
        foreach (BaseAsymProperty asymProperty in _asymProperties)
        {
            asymProperty.ApplyPerspectiveProfile(profile);
        }
    }
}