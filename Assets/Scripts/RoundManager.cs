using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public int numberOfRounds = 3;
    public TextMeshProUGUI timerText;

    private int currentRound = 1; // we will be counting from 1 for rounds
    private SpawnManager spawnmanager;

    public float roundLength = 20;  //seconds
    private float timeRemaining;
    private int minutes;
    private int seconds;

    void Start()
    {
        timeRemaining = roundLength;
        SpawnManager spawnManager = GetComponent<SpawnManager>();
        spawnManager.haltSpawning = false;
    }


    void Update()
    {
        timeRemaining -= Time.deltaTime;
        minutes = Mathf.FloorToInt(timeRemaining / 60f);
        seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);
        timerText.text = string.Format("Time Remaining \n\r {0:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 0)
        {
            RoundRestart();
        }
        else if (currentRound <= 2)
        {

        }
    }

    void RoundRestart()
    {

    }
}
