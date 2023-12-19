using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public List<EnemyToSpawn> enemies = new List<EnemyToSpawn>();
    public int currWave;
    private int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Transform[] spawnLocation;
    public int spawnIndex;
    private float time;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public float spawnTimer;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private TRIGGER_STATE state;
    [SerializeField] CameraFollow cameraFollow;

    
    // Start is called before the first frame update
    private enum TRIGGER_STATE
    {
        IDLE,
        ENTERED
    }

    public void OnAwake()
    {
        state = TRIGGER_STATE.IDLE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        time += Time.deltaTime;

        if (time >= spawnTimer)
        {
            //spawn an enemy
            if (enemiesToSpawn.Count > 0)
            {
                GameObject enemy = (GameObject)Instantiate(enemiesToSpawn[0], GetSpawnPosition().position, Quaternion.identity); // spawn first enemy in our list
                enemiesToSpawn.RemoveAt(0); // and remove it
                spawnedEnemies.Add(enemy);
                time = 0f;
            }
           
        }
        if (cameraFollow.isWaveInProgress)
        {
            List<GameObject> enemiesToRemove = new List<GameObject>();
         
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy == null)
                {
                    enemiesToRemove.Add(enemy);
                }
            }

            foreach (GameObject enemyToRemove in enemiesToRemove)
            {
                spawnedEnemies.Remove(enemyToRemove);
            }

            if(spawnedEnemies.Count == 0 && enemiesToSpawn.Count == 0)
            {
                cameraFollow.isWaveInProgress = false;
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (state == TRIGGER_STATE.IDLE)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("ZAIDEJAS TRIGGERINO LEVEL 1");
                cameraFollow.isWaveInProgress = true;
                state = TRIGGER_STATE.ENTERED;
                GenerateWave();
            }
        }
    }

    public void GenerateWave()
    {
        waveValue = currWave * 100;

        GenerateEnemies();

       
        //waveTimer = waveDuration; // wave duration is read only
    }

    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate
        // 
        // in a loop grab a random enemy 
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.

        // repeat... 

        //  -> if we have no points left, leave the loop

        List<GameObject> generatedEnemies = new List<GameObject>();

        while (waveValue > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int EnemyCost = enemies[randEnemyId].cost;

            if (waveValue - EnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= EnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }
    private Transform GetSpawnPosition()
    {
        return spawnLocation[(Random.Range(0, spawnLocation.Length))];
    }
}


[System.Serializable]
public class EnemyToSpawn
{
    public GameObject enemyPrefab;
    public int cost;
}

