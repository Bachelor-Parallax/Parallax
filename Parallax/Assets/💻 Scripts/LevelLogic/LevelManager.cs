using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Inspector Values

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 humanSpawnPos;

    [SerializeField] private Vector3 catSpawnPos;

    [Header("Stopwatch")]
    [SerializeField] private string elapsedTime;

    #endregion Inspector Values

    private Stopwatch _stopWatch;
    private GameObject _human, _cat;
    private IPerspectiveManager _perspectiveManager;

    private void Start()
    {
        _stopWatch = new Stopwatch();

        // Find the perspective manager in the scene
        _perspectiveManager = UnityExtensions.FindObjectsAssignableTo<IPerspectiveManager>(FindObjectsSortMode.None).FirstOrDefault();
        if (_perspectiveManager == null)
        {
            UnityEngine.Debug.LogError("No perspective manager could be found in the scene.");
            return;
        }

        // Perform initial level setup
        _perspectiveManager.ApplyPerspective();
        FetchPlayers();
        PositionPlayers();

        // As the very last thing, start the stopwatch
        _stopWatch.Start();
    }

    private void FixedUpdate()
    {
        TimeSpan ts = _stopWatch.Elapsed;
        elapsedTime = $"{(int)ts.TotalHours}:{ts.Minutes}:{ts.Seconds},{ts.Milliseconds}";
    }

    /// <summary>
    /// Find the player objects in the scene
    /// </summary>
    private void FetchPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(GameConstants.PLAYER_TAG);
        
        foreach (var p in players)
        {
            if (!p.TryGetComponent<RoleController>(out var rc))
                continue;

            if (rc.role.Value == CharacterRole.Human)
                _human = p;
            else if (rc.role.Value == CharacterRole.Cat)
                _cat = p;
        }
    }

    /// <summary>
    /// Place players in spawn locations
    /// </summary>
    // private void PositionPlayers()
    // {
    //     _human.transform.position = humanSpawnPos;
    //     UnityEngine.Debug.Log("Human spawned on coords: " + _human.transform.position);
    //     _cat.transform.position = catSpawnPos;
    //     UnityEngine.Debug.Log("Cat spawned on coords: " + _cat.transform.position);
    // }
    
    private void PositionPlayers()
    {
        SetPlayerPosition(_human, humanSpawnPos);
        SetPlayerPosition(_cat, catSpawnPos);
    }

    private void SetPlayerPosition(GameObject player, Vector3 pos)
    {
        player.GetComponent<TemporaryMovement>().Teleport(pos);

        if (player.TryGetComponent<TemporaryMovement>(out var movement))
        {
            movement.ResetVerticalVelocity();
        }
    }
}