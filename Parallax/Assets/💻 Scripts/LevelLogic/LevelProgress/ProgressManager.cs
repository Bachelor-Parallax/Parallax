using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class ProgressManager
{
    private static string path => Application.persistentDataPath + "/progress.dat";
    
    public static ProgressData Data = new();

    private static Dictionary<string, LevelProgress> levelLookup = new();

    public static void Save()
    {
        string json = JsonUtility.ToJson(Data, true);
        string encrypted = Encrypt(json);
        
        File.WriteAllText(path, encrypted);
    }

    public static void Load()
    {
        if (!File.Exists(path))
        {
            Data = new ProgressData();
            levelLookup = new Dictionary<string, LevelProgress>();
            return;
        }
        
        string encrypted = File.ReadAllText(path);
        string json = Decrypt(encrypted);
        
        Data = JsonUtility.FromJson<ProgressData>(json);
        
        levelLookup = new Dictionary<string, LevelProgress>();
        
        foreach (var level in Data.levels)
            levelLookup.Add(level.levelName, level);
    }

    static string Encrypt(string text)
    {
        byte key = 129;
        byte[] bytes = Encoding.UTF8.GetBytes(text);

        for (int i = 0; i < bytes.Length; i++)
            bytes[i] ^= key;
        
        return Convert.ToBase64String(bytes);
    }

    static string Decrypt(string encrypted)
    {
        byte key = 129;
        byte[] bytes = Convert.FromBase64String(encrypted);
        
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] ^= key;
        
        return Encoding.UTF8.GetString(bytes);
    }
    
    public static LevelProgress GetLevel(string levelName)
    {
        if (levelLookup.TryGetValue(levelName, out LevelProgress level))
            return level;
        
        level = new LevelProgress();
        level.levelName = levelName;
        
        Data.levels.Add(level);
        levelLookup.Add(levelName, level);
        
        return level;
    }
    
    public static void RegisterLevelCompletion(
        string levelName,
        CharacterRole role,
        float completionTime,
        float devTime)
    {
        LevelProgress level = GetLevel(levelName);

        bool changed = false;

        // BEST TIME
        if (completionTime < level.bestTime)
        {
            Debug.Log($"Saving best time {completionTime} for {levelName}");
            level.bestTime = completionTime;
            changed = true;
        }

        // ROLE TROPHIES
        if (role == CharacterRole.Cat && !level.catTrophyAcquired)
        {
            level.catTrophyAcquired = true;
            changed = true;
        }

        if (role == CharacterRole.Human && !level.humanTrophyAcquired)
        {
            level.humanTrophyAcquired = true;
            changed = true;
        }

        // DEV TIME TROPHY
        if (completionTime <= devTime && !level.devTimeTrophyAcquired)
        {
            level.devTimeTrophyAcquired = true;
            changed = true;
        }

        if (changed)
            Save();
    }

}
