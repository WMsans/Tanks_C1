using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<int, LevelGenerator> levels;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private ItemSpawner itemSpawner;
    private int currentWaveNum;
    private int currentEnemyNum;
    public void SetCurrentWaveNum(int n) => currentWaveNum = n;
    public bool CanGenerate { get; set; } = false;
    private void Update()
    {
        if (currentEnemyNum <= 0 && currentWaveNum <= levels.Count && CanGenerate) GoToNextLevel();
    }

    private void GoToNextLevel()
    {
        if(!CanGenerate) return;
        currentWaveNum++;
        if(currentWaveNum > levels.Count) return;
        levels[currentWaveNum].GenerateLevel(enemySpawner, itemSpawner);
        currentEnemyNum = levels[currentWaveNum].GetEnemyNum();
        EnemyCountText.Instance.UpdateText(currentEnemyNum.ToString());
        StartCoroutine(AddEnemyDieEventCoroutine());
        
        // Save level
        /*GameDataSystemManager.Instance.CurrentGameData.levelProgressList[currentWaveNum].isCleared = true;
        GameDataSystemManager.Instance.SaveProgress();*/
    }

    private IEnumerator AddEnemyDieEventCoroutine()
    {
        yield return new WaitForEndOfFrame();
        var enemyDamagables = FindObjectsByType<EnemyDamagable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var enemy in enemyDamagables)
        {
            enemy.onDeath.AddListener(() => OnEnemyDie(enemy.gameObject));
        }
    }

    public void OnEnemyDie(GameObject enemy)
    {
        currentEnemyNum--;
        EnemyCountText.Instance.UpdateText(currentEnemyNum.ToString());
    }
}
