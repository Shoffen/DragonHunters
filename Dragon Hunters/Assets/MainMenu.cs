using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Tag for the player object
    public string playerTag = "Player";

    public void PlayGame()
    {
        ResetGame();
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void SavePlayer()
    {
        // Save player data
        PlayerController playerController = FindPlayerController();
        if (playerController != null)
        {
            SaveSystem.SavePlayer(playerController);
        }
        else
        {
            Debug.LogError("Player object not found with tag: " + playerTag);
        }
    }

    public void LoadPlayer()
    {
        // Register a callback for when the scene is loaded
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the game scene if it's not already loaded
        if (!UnityEngine.SceneManagement.SceneManager.GetSceneByName("SampleScene").isLoaded)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
        else
        {
            // If scene is already loaded, directly load player data
            LoadPlayerData();
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Check if the loaded scene is the game scene
        if (scene.name == "SampleScene")
        {
            // Unregister the callback to prevent multiple calls
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

            // Load player data once the scene is loaded
            LoadPlayerData();
        }
    }

    private void LoadPlayerData()
    {
        // Find the player object and load player data
        PlayerController playerController = FindPlayerController();
        if (playerController != null)
        {
            playerController.LoadPlayer();
        }
        else
        {
            Debug.LogError("Player object not found with tag: " + playerTag);
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

    private void ResetGame()
    {
        // Reset the game to its initial state
        // Here you can add any logic to reset game-specific data
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
