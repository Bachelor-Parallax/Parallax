public static class GameConstants
{
    public const string PLAYER_TAG = "Player";
    public const string LOBBY_SCENE_NAME = "PlayableLobby";
#if UNITY_EDITOR
    public const int MAX_PLAYERS = 2;
#else
    public const int MAX_PLAYERS = 2;
#endif
}