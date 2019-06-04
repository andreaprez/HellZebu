using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumNamespace;


public class SpawnManager : MonoBehaviour
{
   

    public SpawnManagerData data;
    public GameObject orbePrefab;
    public GameObject calaveraFuegoPrefab;
    public GameObject calaveraHieloPrefab;
    public GameObject cacoDemonPrefab;
    public GameObject wurmLordPrefab;
    public GameObject elitePrefab;

    public GameObject fireWorld;
    public GameObject iceWorld;

    public bool spawnInFireWorld;
    public bool spawnInIceWorld;

    float timeBetweenWavesTimer;
    int maxWaves;
    int currentWave;

    float waitVictoryTime;
    float waitVictoryTimer;
    bool allEnemiesDeathCheck1;
    bool allEnemiesDeath;
    public GameObject unlockedDoor;

    public Transform[] particlePoints;
    public GameObject particle;


    //Fix spawnear particulas solo en los puntos de spawn de cada wave
  public  Dictionary<int, List<Vector3>> spawnPoints;

    public bool checkIsRepeated(int wave, int enemy)
    {
        for(int h = 0; h < spawnPoints[wave].Count; h++)
        {

            if (data.waves[wave].enemies[enemy].spawnPosition == spawnPoints[wave][h])
            {
                return true;
            }
        }
        return false;
    }
    public bool start;
    void Start()
    {
        allEnemiesDeath = false;
        waitVictoryTime = 5f;
        currentWave = 0;
  
        maxWaves = data.wavesAmount;

        spawnPoints = new Dictionary<int, List<Vector3>>();
        timeBetweenWavesTimer = data.waves[currentWave].timeBetweenWaves;

        //Coger puntos de spawn usados
        for(int l = 0; l < maxWaves; l++)
        {
            spawnPoints.Add(l, new List<Vector3>());
        }

        for(int i=0; i<maxWaves; i++)
        {
        
            for(int e = 0; e < data.waves[i].enemies.Length; e++)
            {
                if (e == 0)
                {
                    spawnPoints[i].Add(data.waves[i].enemies[e].spawnPosition);
                }
                else
                {
                    if(checkIsRepeated(i, e))
                    { }
                    else
                    {
                        spawnPoints[i].Add(data.waves[i].enemies[e].spawnPosition);
                    }
                }



            }
        }






    }
    void Update()
    {
       
        if (start)
        {
            if (currentWave < maxWaves)
            {
                timeBetweenWavesTimer -= Time.deltaTime;
                if (timeBetweenWavesTimer <= 0)
                {
                    
                    StartCoroutine(SpawnWave(currentWave));
                    foreach(Vector3 vec in spawnPoints[currentWave])
                    {
                        Instantiate(particle, vec, Quaternion.identity);
                    }
                    currentWave++;
                    if (currentWave < maxWaves)
                    {
                        timeBetweenWavesTimer = data.waves[currentWave].timeBetweenWaves;
                    }

                }
            }
            else
            {
                //Doble checkear por si queda algun enemigo en la coorutina por aparecer
                Enemy[] array1 = GameObject.FindObjectsOfType<Enemy>();
                if (array1.Length == 0 || !array1[0].enabled)
                    allEnemiesDeathCheck1 = true;

                if (allEnemiesDeathCheck1)
                {
                    waitVictoryTimer -= Time.deltaTime;
                    if (waitVictoryTimer <= 0)
                    {
                        Enemy[] array2 = GameObject.FindObjectsOfType<Enemy>();
                        if (array2.Length == 0 || !array2[0].enabled){
                            allEnemiesDeath = true;
                            unlockedDoor.SetActive(true);
                        }
                        else
                        {
                            allEnemiesDeathCheck1 = false;
                            waitVictoryTimer = waitVictoryTime;
                        }
                    }
                }
            }
        }
    }


    IEnumerator SpawnWave(int wave)
    {
        for(int i=0;i<data.waves[wave].enemies.Length; i++)
        {
            SpawnEnemy(data.waves[wave].enemies[i]);
            yield return new WaitForSeconds(data.waves[wave].spawnTimeBetweenEnemies);
        }
        yield break;

    }

    



    public void SpawnEnemy(EnemeiesWaveClass e)
    {
        switch (e.type)
        {
            case EnemiesTypes.ORBE:
                Spawn(orbePrefab, e);
                break;
            case EnemiesTypes.CALAVERAFUEGO:
                Spawn(calaveraFuegoPrefab, e);
                break;
            case EnemiesTypes.CALAVERAHIELO:
                Spawn(calaveraHieloPrefab, e);
                break;
            case EnemiesTypes.CACODEMON:
                Spawn(cacoDemonPrefab, e);
                break;
            case EnemiesTypes.WURMLORD:
                Spawn(wurmLordPrefab, e);
                break;
            case EnemiesTypes.ELITE:
                Spawn(elitePrefab, e);
                break;

        }
    }
    public void Spawn(GameObject prefab, EnemeiesWaveClass e)
    {
        if (spawnInFireWorld && spawnInIceWorld == false)
        {
            GameObject tEnemy = Instantiate(prefab, e.spawnPosition, Quaternion.identity);
            tEnemy.GetComponent<Enemy>().InitialWorld = Enemy.EWorld.FIRE;

        }
        else if (spawnInIceWorld && spawnInFireWorld == false)
        {
            GameObject tEnemy = Instantiate(prefab, e.spawnPosition, Quaternion.identity);
            tEnemy.GetComponent<Enemy>().InitialWorld = Enemy.EWorld.ICE;

        }
        else if (spawnInFireWorld && spawnInIceWorld)
        {
            if (prefab.name == "Empty")
            {

            }
            else
            {

                if (prefab.name != "SkullFire" && prefab.name != "Centipede")
                {
                    GameObject tEnemy2 = Instantiate(prefab, e.spawnPosition, Quaternion.identity);
                    tEnemy2.GetComponent<Enemy>().InitialWorld = Enemy.EWorld.ICE;
                }
                if (prefab.name != "skullICE")
                {
                    GameObject tEnemy = Instantiate(prefab, e.spawnPosition, Quaternion.identity);
                    tEnemy.GetComponent<Enemy>().InitialWorld = Enemy.EWorld.FIRE;
                }
            }

            

        }
    }
}

