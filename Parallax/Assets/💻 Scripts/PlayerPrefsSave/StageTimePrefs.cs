using UnityEngine;

public static class StageTimePrefs
{
    private static string Key(string stageName)
    {
        stageName = (stageName ?? "").Trim();
        stageName = stageName.Replace(" ", "_");
        return "stage_time_" + stageName;
    }

    // Call from any script:
    // StageTimePrefs.SaveStageTime("Stage1", 32.51); - Stage name and time in seconds
    public static void SaveStageTime(string stageName, double timeSeconds)
    {
        string key = Key(stageName);

        // PlayerPrefs doesn't support double; store as float.
        float t = (float)timeSeconds;

        if (PlayerPrefs.HasKey(key))
        {
            float best = PlayerPrefs.GetFloat(key);
            if (t < best)
                PlayerPrefs.SetFloat(key, t);
        }
        else
        {
            PlayerPrefs.SetFloat(key, t);
        }

        PlayerPrefs.Save();
    }

    public static double GetBestTime(string stageName)
    {
        string key = Key(stageName);
        if (!PlayerPrefs.HasKey(key)) return -1;
        return PlayerPrefs.GetFloat(key);
    }

    public static bool HasTime(string stageName)
    {
        return PlayerPrefs.HasKey(Key(stageName));
    }
}