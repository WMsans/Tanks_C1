using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private SerializedDictionary<int, LevelGenerator> levels;
    [SerializeField] private TextMeshProUGUI enemyCntText;
    private int currentWaveNum;
    private int currentEnemyNum;
    public void SetCurrentWaveNum(int n) => currentWaveNum = n;
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
        enemyCntText.text = currentEnemyNum.ToString();
        
        // Save level
        GameDataSystemManager.Instance.CurrentGameData.levelProgressList[currentWaveNum].isCleared = true;
        GameDataSystemManager.Instance.SaveProgress();
    }

    public void OnEnemyDie(GameObject enemy)
    {
        currentEnemyNum--;
        enemyCntText.text = currentEnemyNum.ToString();
    }
}
