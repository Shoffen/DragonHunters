using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyData
{
    
    public string prefabName; // Store the prefab name
    public float[] position;
    public float health;
   
    public EnemyData()
    {
        position = new float[3];
        
    }
}
