using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelData", menuName = "Lobby/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Preset data")]
    public string levelName;
    public string levelDescription;
    public string sceneName;
    public string devTime;
    public Material levelImage;
    
    [Header("Player data")]
    public string bestTime;
    public bool catTrophy;
    public bool humanTrophy;
    public bool beatDevTrophy;
}