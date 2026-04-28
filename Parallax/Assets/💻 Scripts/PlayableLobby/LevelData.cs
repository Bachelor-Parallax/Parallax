using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelData", menuName = "Lobby/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Preset data")]
    public string levelName;
    public string levelDescription;
    public string sceneName;
    public float devTime;
    public Sprite levelImage;
}