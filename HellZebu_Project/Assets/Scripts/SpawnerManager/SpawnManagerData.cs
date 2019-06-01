using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumNamespace;
[CreateAssetMenu(fileName = "DefaultSpawnManagerConfig", menuName = "Create spawn manager config")]
public class SpawnManagerData : ScriptableObject
{
    public int wavesAmount;
    public Wave[] waves;


}
[System.Serializable]
public class Wave
{
    public float timeBetweenWaves;
    public float spawnTimeBetweenEnemies;
    public int enemiesAmount;
    public EnemeiesWaveClass[] enemies;
}
[System.Serializable]
public class EnemeiesWaveClass
{
    public int spawnPointID=0;
    public EnemiesTypes type;
    public Vector3 spawnPosition;
}