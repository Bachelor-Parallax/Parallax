using TMPro;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    public LevelData levelData;
    
    [Header("UI")]
    public TextMeshPro levelNameText;
    public TextMeshPro bestTimeText;
    public TextMeshPro devTimeText;
    public SpriteRenderer image;
    public GameObject catTrophyObject;
    public GameObject humanTrophyObject;
    public GameObject devTimeTrophyObject;

    [Header("Voting")]
    public VoteZoneTrigger voteZone;
    
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (levelData == null) return;

        levelNameText.text = levelData.levelName;

        LevelProgress progress = ProgressManager.GetLevel(levelData.levelName);
        
        bestTimeText.text = FormatTime(progress.bestTime);
        devTimeText.text = FormatTime(levelData.devTime);
        
        image.sprite = levelData.levelImage;
        
        catTrophyObject.SetActive(progress.catTrophyAcquired);
        humanTrophyObject.SetActive(progress.humanTrophyAcquired);
        devTimeTrophyObject.SetActive(progress.devTimeTrophyAcquired);

        voteZone.SetLevel(levelData);
    }

    string FormatTime(float time)
    {
        if (time == float.MaxValue)
            return "--:--";
        
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        int milliseconds = Mathf.FloorToInt(time % 1000);
        
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}
