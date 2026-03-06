using System.Linq;
using UnityEngine;

/// <summary>
/// Debug-only tool that simulates split screen by forcing
/// BaseAsymProperty instances to apply a perspective each render.
/// Does NOT affect runtime architecture.
///
/// Tool spawns two new cameras, taking up half the screen each.
/// Adding this script to the scene is all you need to enable
/// split screen features.
///
/// NOTE: This is very inefficient and WILL impact the frame-rate
/// negatively.
/// </summary>
public class SplitScreenTool : MonoBehaviour
{
    #region Inspector Values

    [Tooltip("Manually switch the main camera's perspective profile")]
    [SerializeField] private PerspectiveProfile _mainProfile;

    #endregion Inspector Values

    // a value used to compare to main profile so changes
    // to the main profile made through the inspector are
    // discovered and applied
    private PerspectiveProfile _current;

    private bool _isSplitScreen;
    private GameObject _mainCamera, _humanCam, _catCam;
    private PerspectiveManager _manager;

    private void Start()
    {
        _manager = FindObjectsByType<PerspectiveManager>(FindObjectsSortMode.None).First();
        _mainCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None).First().gameObject;

        // create cameras for split screen
        _humanCam = SpawnCamera(PerspectiveProfile.Human);
        _catCam = SpawnCamera(PerspectiveProfile.Cat);

        // apply selected perspective
        _current = _mainProfile;
        _manager.ApplyPerspective(_mainProfile);
    }

    private void Update()
    {
        if (!_isSplitScreen && _current != _mainProfile)
        {
            _manager.ApplyPerspective(_mainProfile);
            _current = _mainProfile;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_isSplitScreen)
            {
                _manager.ApplyPerspective(_mainProfile);
                _mainCamera.SetActive(true);
                _humanCam.SetActive(false);
                _catCam.SetActive(false);
                _isSplitScreen = false;
            }
            else
            {
                _mainCamera.SetActive(false);
                _humanCam.SetActive(true);
                _catCam.SetActive(true);
                _isSplitScreen = true;
            }
        }
    }

    /// <summary>
    /// Creates a camera that takes up half the screen
    /// </summary>
    private GameObject SpawnCamera(PerspectiveProfile p)
    {
        string profileName = p == PerspectiveProfile.Human ? "Human" : "Cat";
        GameObject camObj = new GameObject($"Split Screen Camera ({profileName})");

        // TODO: positioning logic should be changed when cameras attach to moving players
        camObj.transform.position = _mainCamera.gameObject.transform.position;
        camObj.transform.rotation = _mainCamera.gameObject.transform.rotation;

        SplitScreenCam splitCam = camObj.AddComponent<SplitScreenCam>();
        splitCam.Profile = p;
        splitCam.Manager = _manager;

        float x = p == PerspectiveProfile.Human ? 0 : 0.5f;
        Camera cam = camObj.AddComponent<Camera>();
        cam.rect = new Rect(x, 0f, 0.5f, 1f);

        camObj.SetActive(false);
        return camObj;
    }

    /// <summary>
    /// A MonoBehaviour for split screen cameras, so they can toggle profiles each render
    /// </summary>
    public class SplitScreenCam : MonoBehaviour
    {
        public PerspectiveProfile Profile;
        public PerspectiveManager Manager;

        private void OnPreCull()
        {
            Manager.ApplyPerspective(Profile);
        }
    }
}