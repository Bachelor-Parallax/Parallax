using System.Collections.Generic;

[System.Serializable]
public class LevelProgress
{
    public string levelName;
    public float bestTime;
    public bool catTrophyAcquired;
    public bool humanTrophyAcquired;
    public bool devTimeTrophyAcquired;
}

[System.Serializable]
public class ProgressData
{
    public List<LevelProgress> levels = new();
}