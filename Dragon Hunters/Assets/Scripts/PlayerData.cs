using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    public int level;
    public int wave;
    public int health;
    public float[] position;
    public float[] cameraPosition;
   
    public PlayerData (PlayerController player)
    {
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


    }
}
