using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private SerializedDictionary<int, LevelGenerator> levels;
    private int currentWaveNum;
    private int currentEnemyNum;
    private void Update()
    {
        if (currentEnemyNum <= 0 && currentWaveNum <= levels.Count) GoToNextLevel();
    }

    private void GoToNextLevel()
    {
        currentWaveNum++;
        if(currentWaveNum > levels.Count) return;
        levels[currentWaveNum].GenerateLevel();
        currentEnemyNum = levels[currentWaveNum].GetEnemyNum();
    }

    public void OnEnemyDie(GameObject enemy)
    {
        currentEnemyNum--;
    }
}
