using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    // BACKGROUND MUSIC HERE
    public GameObject musicObject;

    public float roundLength;  //seconds
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI displayText;
    public float fadeDelay;
    public float displayTextFadeOut = 0.25f;
    public PlayerUIManager playerUImanager;
    public PlayerCombat playerCombat;
    public GameObject character;
    public UpgradeController upgradeController;

    [SerializeField] private float timeRemaining; //debugging purposes etc
    private SpawnManager spawnManager;
    [SerializeField] private int currentRound = 1; // we will be counting from 1 for rounds
    private int minutes;
    private int seconds;

    public int enemyRoundCount;
    public int enemyLeftToSpawn;
    private int[] enemiesRemainingText = new int[4]; // blue orange purple green

    private int constantC = 5;
    private int constantN = 20; // see desmos


    IEnumerator Start()
    {
        RoundCountUpdate(out enemyRoundCount);
        enemyLeftToSpawn = enemyRoundCount;
        timeRemaining = roundLength;
        spawnManager = GetComponent<SpawnManager>();
        yield return new WaitForSeconds(3);
        StartCoroutine(spawnManager.StartSpawning(enemyRoundCount));

        displayText.text = currentRound.ToString();
        displayText.enabled = true;
        Invoke(nameof(startRoundDisplayCoroutine), fadeDelay); // show round at full opacity for some time before fading

        musicObject.GetComponent<AudioSource>().enabled = true;
    }


    void startRoundDisplayCoroutine()
    {
        StartCoroutine(RoundDisplay(currentRound.ToString(), displayTextFadeOut));
    }


    void RoundCountUpdate(out int enemyRoundCount)
    {
        var x = Mathf.Pow(currentRound + 5, 2);
        enemyRoundCount = (int)Mathf.Round((1.0f / constantN * x) + constantC - Mathf.Log(currentRound));
    }


    public IEnumerator RoundDisplay(string text, float time)
    {
        while (displayText.color.a > 0.0f)
        {
            displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, displayText.color.a - (Time.deltaTime / time));
            yield return null;
        }
        displayText.enabled = false;
    }


    void Update()
    {
        timeRemaining -= Time.deltaTime;
        minutes = Mathf.FloorToInt(timeRemaining / 60f);
        seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);
        timerText.text = string.Format("Time Remaining \n\r {0:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 0)
        {
            RoundFail();
        }
    }


    public void RoundFail()
    {
        var blocked = false;

        if (blocked)
        {
            return;
        }

        blocked = true;

        spawnManager.StopAllCoroutines();
        StopAllCoroutines();
        timeRemaining = 0;
        RoundCountUpdate(out enemyRoundCount);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        //character.GetComponent<AudioSource>().Play(); - maybe one day
        playerUImanager.GameOverDisplay(playerCombat.kills, currentRound);
        enemyLeftToSpawn = 0;

        StartCoroutine(EndGame());
    }


    private IEnumerator EndGame()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
                yield break;
            }
            yield return null;
        }
    }


    public void RoundRestart()
    {
        timeRemaining = roundLength;
        currentRound++;
        RoundCountUpdate(out enemyRoundCount);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        displayText.text = currentRound.ToString();
        displayText.enabled = true;
        displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, 1);
        Invoke(nameof(startRoundDisplayCoroutine), fadeDelay); // show round at full opacity for some time before fading

        enemyLeftToSpawn = enemyRoundCount;
        StartCoroutine(spawnManager.StartSpawning(enemyRoundCount));
    }
}
