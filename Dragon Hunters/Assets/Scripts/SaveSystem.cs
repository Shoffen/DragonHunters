using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(PlayerController player, List<GameObject> remainingEnemies, List<GameObject> leftToSpawn, bool wave, bool trigger)
    {
        Debug.Log("OOOOOOOOOOOOOOOOOOOOOOOOOOOO: " + leftToSpawn.Count);
        
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.ba+";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData playerData = new PlayerData(player, remainingEnemies, leftToSpawn, wave, trigger);
        Debug.Log("WAVE STATUSAS SAUGOJANT: " + playerData.isWaveInProgress);
        Debug.Log("COLLIDER STATUSAS SAUGOJANT: " + playerData.isBoxColliderTrigger);
        formatter.Serialize(stream, playerData);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {


        string path = Application.persistentDataPath + "/player.ba+";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            Debug.Log("OOOOOOOOOOOOOOOOOOOOOOOOOOOO: " + data.leftToSpawnData.Count);
            Debug.Log("WAVE STATUSAS SAUGOJANT: " + data.isWaveInProgress);
            Debug.Log("COLLIDER STATUSAS SAUGOJANT: " + data.isBoxColliderTrigger);
            
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in: " + path);
            return null;
        }
    }
}
