using System;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Scene = UnityEditor.SearchService.Scene;

public class LevelManager : MonoBehaviour
{
    #region Inspector Values

    [Header("Spawn Positions")]
    [SerializeField] private Vector3 humanSpawnPos;

    [SerializeField] private Vector3 catSpawnPos;

    #endregion Inspector Values

    public TimeSpan ElapsedTime
    {
        get
        {
            if (_stopWatch is { IsRunning: true })
            {
                return _stopWatch.Elapsed;
            }
            return TimeSpan.Zero;
        }
    }

    private Stopwatch _stopWatch;
    private GameObject _human, _cat;

    private PerspectiveManager _perspectiveManager;
    private NetworkManager _networkManager;

    public static LevelManager Instance => _instance;
    private static LevelManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;

        _networkManager = FindFirstObjectByType<NetworkManager>();
        if (_networkManager == null)
        {
            UnityEngine.Debug.LogError($"No {nameof(NetworkManager)} found in scene.");
        }

        _perspectiveManager = FindFirstObjectByType<PerspectiveManager>();
        if (_perspectiveManager == null)
        {
            UnityEngine.Debug.LogError($"No {nameof(PerspectiveManager)} found in scene.");
        }
    }

    private void Start()
    {
        _stopWatch = new Stopwatch();

        // Perform initial level setup
        _perspectiveManager.ApplyPerspective(DetermineLocalRole());
        FetchPlayers();
        PositionPlayers();

        // As the very last thing, start the stopwatch
        _stopWatch.Start();
    }

    /// <summary>
    /// Reloads the current scene, causing a complete reset of the level.
    /// Player's will be repositioned as the level manager re-runs the setup in start
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Saves level data and loads the lobby
    /// </summary>
    public void CompleteLevel()
    {
        // TODO: Save the completion time and handle trophies
        ExitLevel();
    }

    /// <summary>
    /// Exits the level by loading the lobby scene
    /// </summary>
    public void ExitLevel()
    {
        SceneManager.LoadScene(GameConstants.LOBBY_SCENE_NAME);
    }

    /// <summary>
    /// Find out which role this game instance is controlling
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private CharacterRole DetermineLocalRole()
    {
        var localClient = _networkManager.LocalClient;
        if (localClient?.PlayerObject == null)
        {
            throw new System.InvalidOperationException(
                "Local player is not spawned yet. This method must only be called after spawn.");
        }

        var playerObject = localClient.PlayerObject;
        return playerObject.GetComponent<RoleController>().role.Value;
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
    /// Position players at spawn locations
    /// </summary>
    private void PositionPlayers()
    {
        // local helper function to position players
        void SetPlayerPosition(GameObject player, Vector3 pos)
        {
            if (player.TryGetComponent<Movement>(out var movement))
            {
                movement.Teleport(pos);
                movement.ResetVerticalVelocity();
            }
        }
        if (_human != null) SetPlayerPosition(_human, humanSpawnPos);
        if (_cat != null) SetPlayerPosition(_cat, catSpawnPos);
    }
}