using TMPro;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    public LevelData levelData;
    
    [Header("UI")]
    public TextMeshPro levelNameText;
    public TextMeshPro bestTimeText;
    public SpriteRenderer image;

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
        bestTimeText.text = levelData.bestTime;
        image.sprite = levelData.levelImage;

        voteZone.SetLevel(levelData);
    }
}
