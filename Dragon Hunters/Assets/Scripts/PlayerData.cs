using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class PlayerData
{
    // Existing variables
    public int level;
    public int wave;
    public int health;
    public float[] position;
    public float[] cameraPosition;
    public List<EnemyData> remainingEnemiesData;
    public List<EnemyData> leftToSpawnData;

    // New variables
    public bool isWaveInProgress;
    public bool isBoxColliderTrigger;

    // Constructor with additional parameters
    public PlayerData(PlayerController player, List<GameObject> remainingEnemies, List<GameObject> leftToSpawn, bool waveInProgress, bool boxColliderTrigger)
    {
        // Assign values for existing variables
        level = player.level;
        health = player.health;
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
        cameraPosition = new float[3];
        cameraPosition[0] = player.mainCamera.transform.position.x;
        cameraPosition[1] = player.mainCamera.transform.position.y;
        cameraPosition[2] = player.mainCamera.transform.position.z;

        // Assign values for new variables
        isWaveInProgress = waveInProgress;
        isBoxColliderTrigger = boxColliderTrigger;

        // Initialize remainingEnemiesData if it's null
        if (leftToSpawnData == null)
        {
            leftToSpawnData = new List<EnemyData>();
        }

        // Update remaining enemies data
        if (leftToSpawn.Count > 0)
        {
            foreach (GameObject enemy in leftToSpawn)
            {
                EnemyData enemyData = new EnemyData();
                enemyData.prefabName = enemy.name.Replace("(Clone)", ""); // Save prefab name
                leftToSpawnData.Add(enemyData);
            }
        }

        // Initialize remainingEnemiesData if it's null
        if (remainingEnemiesData == null)
        {
            remainingEnemiesData = new List<EnemyData>();
        }

        // Update remaining enemies data
        if (remainingEnemies.Count > 0)
        {
            foreach (GameObject enemy in remainingEnemies)
            {
                EnemyData enemyData = new EnemyData();
                enemyData.prefabName = enemy.name.Replace("(Clone)", ""); // Save prefab name
                enemyData.position = new float[3];
                enemyData.position[0] = enemy.transform.position.x;
                enemyData.position[1] = enemy.transform.position.y;
                enemyData.position[2] = enemy.transform.position.z;
                remainingEnemiesData.Add(enemyData);
            }
        }
    }
}


