using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthenticationServiceWrapper : PersistentSingleton<AuthenticationServiceWrapper>
{
    private string _playerId = "";
    private string _playerName = "";

    public async Task<string> Authenticate()
    {
        string playerId = "";
        playerId = await Authenticate("Player" + Random.Range(0, 1000));
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
