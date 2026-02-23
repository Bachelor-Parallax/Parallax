using System.Linq;
using UnityEngine;

/// <summary>
/// Debug-only tool that simulates split screen by forcing
/// BaseAsymComponent instances to apply a perspective each render.
/// Does NOT affect runtime architecture.
///
/// Tool spawns two new cameras, taking up half the screen each.
/// Adding this script to the scene is all you need to enable
/// split screen features.
/// </summary>
///
public class SplitScreenTool : MonoBehaviour
{
    public PerspectiveProfile Profile;
    private PerspectiveProfile _current;

    private bool _isSplitScreen;
    private GameObject _mainCamera, _camA, _camB;
    private PerspectiveManager _manager;

    private void Start()
    {
        _manager = FindObjectsByType<PerspectiveManager>(FindObjectsSortMode.None).First();

        // find the main camera
        _mainCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None).First().gameObject;
        _camA = SpawnCamera(PerspectiveProfile.A);
        _camB = SpawnCamera(PerspectiveProfile.B);

        _current = Profile;
        _manager.ApplyPerspective(Profile);
    }

    private void Update()
    {
        if (!_isSplitScreen && _current != Profile)
        {
            _manager.ApplyPerspective(Profile);
            _current = Profile;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_isSplitScreen)
            {
                _manager.ApplyPerspective(Profile);
                _mainCamera.SetActive(true);
                _camA.SetActive(false);
                _camB.SetActive(false);
                _isSplitScreen = false;
            }
            else
            {
                _mainCamera.SetActive(false);
                _camA.SetActive(true);
                _camB.SetActive(true);
                _isSplitScreen = true;
            }
        }
    }

    /// <summary>
    /// Creates a camera that takes up half the screen
    /// </summary>
    private GameObject SpawnCamera(PerspectiveProfile p)
    {
        GameObject camObj = new GameObject("Split Screen Camera");

        // TODO: positioning logic should be changed when cameras attach to moving players
        camObj.transform.position = _mainCamera.gameObject.transform.position;
        camObj.transform.rotation = _mainCamera.gameObject.transform.rotation;

        SplitScreenCam splitCam = camObj.AddComponent<SplitScreenCam>();
        splitCam.Profile = p;
        splitCam.Man = _manager;

        float x = p == PerspectiveProfile.A ? 0 : 0.5f;
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
        public PerspectiveManager Man;

        private void OnPreCull()
        {
            Man.ApplyPerspective(Profile);
        }
    }
}