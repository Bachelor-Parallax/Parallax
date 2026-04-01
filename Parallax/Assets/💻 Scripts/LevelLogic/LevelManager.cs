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
        _human = GameObject.FindGameObjectWithTag(GameConstants.HUMAN_TAG);
        if (_human == null) UnityEngine.Debug.LogError($"No GameObject with tag {GameConstants.HUMAN_TAG}");

        _cat = GameObject.FindGameObjectWithTag(GameConstants.CAT_TAG);
        if (_cat == null) UnityEngine.Debug.LogError($"No GameObject with tag {GameConstants.CAT_TAG}");
    }

    /// <summary>
    /// Place players in spawn locations
    /// </summary>
    private void PositionPlayers()
    {
        _human.transform.position = humanSpawnPos;
        _cat.transform.position = catSpawnPos;
    }
}