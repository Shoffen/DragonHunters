using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TaskLab1 : MonoBehaviour
{
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesText;
    public WaveSpawner waveSpawner;

    // Start is called before the first frame update
    void Start()
    {
        waveText.text = "Wave no: " + waveSpawner.currWave.ToString();
        enemiesText.text = "Enemies left: " + waveSpawner.spawnedEnemies.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        waveText.text = "Wave no: " + waveSpawner.currWave.ToString();
        enemiesText.text = "Enemies left: " + waveSpawner.spawnedEnemies.Count.ToString();
    }
}
