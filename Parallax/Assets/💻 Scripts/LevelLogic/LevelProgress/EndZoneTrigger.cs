using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EndZoneTrigger : NetworkBehaviour
{
    [SerializeField] private LevelData levelData;
    
    private LevelManager _levelManager;

    private string _levelName;
    private float _devTime;
    private bool _completed;

    private HashSet<CharacterRole> _playersInZone = new();

    private void Awake()
    {
        _levelName = levelData.levelName;
        _devTime = levelData.devTime;
        
        _levelManager = FindFirstObjectByType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (!other.TryGetComponent<RoleController>(out var roleController))
            return;

        CharacterRole role = roleController.role.Value;
        
        _playersInZone.Add(role);

        if (_playersInZone.Count == 2 && !_completed)
        {
            _completed = true;
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        float time = (float)_levelManager.ElapsedTime.TotalSeconds;
        
        LevelCompletedClientRpc(time);
        
        SceneLoader.Instance.LoadGameScene("PlayableLobby");
    }
    
    [ClientRpc]
    private void LevelCompletedClientRpc(float time)
    {
        var levelManager = FindFirstObjectByType<LevelManager>();

        CharacterRole myRole = levelManager.DetermineLocalRole();

        ProgressManager.RegisterLevelCompletion(
            _levelName,
            myRole,
            time,
            _devTime
        );
    }
}
