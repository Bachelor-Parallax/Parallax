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
        bestTimeText.text = levelData.bestTime;
        devTimeText.text = levelData.devTime;
        image.sprite = levelData.levelImage;
        
        catTrophyObject.SetActive(levelData.catTrophy);
        humanTrophyObject.SetActive(levelData.humanTrophy);
        devTimeTrophyObject.SetActive(levelData.devTimeTrophy);

        voteZone.SetLevel(levelData);
    }
}
