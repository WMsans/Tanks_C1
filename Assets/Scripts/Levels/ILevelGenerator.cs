using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelGenerator
{
    public string LevelName { get; }
    public void GenerateLevel(EnemySpawner enemySpawner, ItemSpawner itemSpawner);
}
