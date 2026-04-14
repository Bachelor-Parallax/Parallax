using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Lobby/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string levelDescription;
    public string bestTime;
    public string sceneName;
}