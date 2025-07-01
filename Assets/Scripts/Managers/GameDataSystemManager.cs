using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataSystemManager : MonoSingleton<GameDataSystemManager>
{
    private GameDataSystem _gameDataSystem;

    public GameData CurrentGameData { get; set; }
    public List<LevelInfo> levelInfos;

    private void Start()
    {
        InitGameDataSystem();
    }

    private void InitGameDataSystem()
    {
        _gameDataSystem = new GameDataSystem(new JsonSerializer(), "json");
        LoadProgress();
        
    }

    public void SaveProgress()
    {
        _gameDataSystem.Save(CurrentGameData);
    }

    public void LoadProgress()
    {
        CurrentGameData = _gameDataSystem.Load("GameData");
    }

    public void ClearProgress()
    {
        _gameDataSystem.Delete(CurrentGameData.name);
    }
    public bool HasProgress()
    {
        bool hasProgress = false;
        foreach (LevelProgress progress in CurrentGameData.levelProgressList)
        {
            if (progress.isCleared)
            {
                hasProgress = true;
                break;
            }
        }
        return hasProgress;
    }
    public bool IsLevelCleared(string levelName)
    {
        return CurrentGameData.levelProgressList.FirstOrDefault(x=>x.levelName == levelName) is { isCleared: true };
    }
}
