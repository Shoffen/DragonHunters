using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string playerTag = "Player";
    private bool isLoading = false;
    private float time = 0f;
    private float spawnTimer = 3f;
    private PlayerController playerController;
    private bool found = false;
    private PlayerData playerData;

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("STARTING");
    }

    public void SavePlayer()
    {
        playerController = FindPlayerController();
        if (playerController != null)
        {
            // Get all existing enemies in the scene
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Create a list to store the alive enemies
            List<GameObject> aliveEnemies = new List<GameObject>();

            // Iterate through all enemies to check if they are still alive
            foreach (GameObject enemy in allEnemies)
            {
                if (enemy != null) // If the enemy exists, add it to the list of alive enemies
                {
                    aliveEnemies.Add(enemy);
                }
            }

            // Save player data with the list of alive enemies
            // Pass the current values of isWaveInProgress and isTrigger
            SaveSystem.SavePlayer(playerController, aliveEnemies, playerController.leftToSpawnEnemies,
                                  playerController.waveSpawner.cameraFollow.isWaveInProgress,
                                  playerController.waveSpawner.boxCollider.enabled);
        }
        else
        {
            Debug.LogError("Player object not found with tag: " + playerTag);
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            LoadPlayerData();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            isLoading = false;
        }
    }

    public void LoadPlayer()
    {
        isLoading = true;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }




    private IEnumerator LoadPlayerDataAfterFrame()
    {
        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();

        // Now load player data
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        if (!isLoading)
        {
            found = false;
            return;
        }

        playerController = FindPlayerController();
        if (playerController != null)
        {
            playerData = SaveSystem.LoadPlayer();
            if (playerData != null)
            {

                // Set isWaveInProgress to the value loaded from saved data
                playerController.mainCamera.GetComponent<CameraFollow>().isWaveInProgress = playerData.isWaveInProgress;

                // Set isTrigger to the value loaded from saved data
                
                playerController.waveSpawner.boxCollider.enabled = playerData.isBoxColliderTrigger;


                //Debug.Log(playerController.waveSpawner.cameraFollow.gameObject.name);
                //Debug.Log("KAAAAAAAAAAAAAAAAS CIA DABAR, STATUSAS: " + playerController.mainCamera.GetComponent<CameraFollow>().isWaveInProgress);
                //Debug.Log("KAAAAAAAAAAAAAAAAS CIA DABAR, SMOOTHING: " + playerController.mainCamera.GetComponent<CameraFollow>().smoothing);

                Debug.Log(playerController.waveSpawner.boxCollider.gameObject.name);
                Debug.Log("KAAAAAAAAAAAAAAAAS CIA DABAR, STATUSAS: " + playerController.waveSpawner.boxCollider.isTrigger);
               


                found = true;
                playerController.level = playerData.level;
                playerController.health = playerData.health;
                Vector3 playerPosition = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
                playerController.transform.position = playerPosition;
                Vector3 cameraPosition = new Vector3(playerData.cameraPosition[0], playerData.cameraPosition[1], playerData.cameraPosition[2]);
                playerController.mainCamera.transform.position = cameraPosition;

                GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in existingEnemies)
                {
                    Destroy(enemy);
                }

                List<GameObject> enemyPrefabsToSpawn = new List<GameObject>();

                if (playerData.leftToSpawnData.Count > 0)
                {
                    // Populate the list of enemy prefabs
                    for (int i = 0; i < playerData.leftToSpawnData.Count; i++)
                    {
                        GameObject enemyPrefab = Resources.Load<GameObject>(playerData.leftToSpawnData[i].prefabName);
                        enemyPrefabsToSpawn.Add(enemyPrefab);
                    }

                    // Send the list of enemy prefabs to the WaveSpawner.cs script
                    playerController.waveSpawner.enemiesToSpawn = enemyPrefabsToSpawn;

                    
                }

                foreach (EnemyData enemyData in playerData.remainingEnemiesData)
                {
                    GameObject enemyPrefab = Resources.Load<GameObject>(enemyData.prefabName);
                    if (enemyPrefab != null)
                    {
                        GameObject enemy = Instantiate(enemyPrefab, new Vector3(enemyData.position[0], enemyData.position[1], enemyData.position[2]), Quaternion.identity);
                        enemy.GetComponent<Enemy>().healthBar.slider.value = enemyData.health;
                    }
                    else
                    {
                        Debug.LogWarning("Failed to load enemy prefab: " + enemyData.prefabName);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Player object not found with tag: " + playerTag);
            found = false;
        }
    }

    private PlayerController FindPlayerController()
    {
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                return playerController;
            }
            else
            {
                Debug.LogError("PlayerController component not found on the player object.");
                return null;
            }
        }
        else
        {
            Debug.LogError("Player object not found with tag: " + playerTag);
            return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
