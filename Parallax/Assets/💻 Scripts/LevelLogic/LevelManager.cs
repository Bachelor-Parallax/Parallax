using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Inspector Values

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 _humanSpawnPos;
    [SerializeField] private Vector3 _catSpawnPos;

    #endregion Inspector Values

    private GameObject _human, _cat;
    private IPerspectiveManager _perspectiveManager;

    private void Awake()
    {
        _perspectiveManager = UnityExtensions.FindObjectsAssignableTo<IPerspectiveManager>(FindObjectsSortMode.None)
            .FirstOrDefault();

        if (_perspectiveManager == null)
        {
            Debug.LogError("No perspective manager could be found in the scene.");
        }
    }

    private void Start()
    {
        FetchPlayers();
        PositionPlayers();
    }

    /// <summary>
    /// Find the player objects in the scene
    /// </summary>
    private void FetchPlayers()
    {
        _human = GameObject.FindGameObjectWithTag("Human");
        if (_human == null) Debug.LogError("No GameObject with tag 'Human'");

        _cat = GameObject.FindGameObjectWithTag("Cat");
        if (_cat == null) Debug.LogError("No GameObject with tag 'Cat'");
    }

    /// <summary>
    /// Place players in spawn locations
    /// </summary>
    private void PositionPlayers()
    {
        _human.transform.position = _humanSpawnPos;
        _cat.transform.position = _catSpawnPos;
    }
}