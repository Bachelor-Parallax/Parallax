using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthenticationServiceWrapper : MonoBehaviour
{
    public static AuthenticationServiceWrapper Instance { get; private set; }

    private string _playerId = "";
    private string _playerName = "";
    
    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<string> Authenticate()
    {
        string playerId = "";
        playerId = await Authenticate("Player" + Random.Range(0, 1000));
        Debug.Log("Player " + _playerName + " authenticated with id: " + playerId);
        return playerId;
    }

    public async Task<string> Authenticate(string playerName)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions options = new();
            options.SetProfile(playerName);
            await UnityServices.InitializeAsync(options);
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _playerId = AuthenticationService.Instance.PlayerId;
            _playerName = playerName;
        }

        return _playerId;
    }
}
