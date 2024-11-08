using System.Collections;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public float roundLength;  //seconds
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI displayText;
    public float displayTextFadeOut = 0.25f;

    [SerializeField] private float timeRemaining; //debugging purposes etc
    private SpawnManager spawnManager;
    private int currentRound = 1; // we will be counting from 1 for rounds
    private int minutes;
    private int seconds;

    void Start()
    {
        timeRemaining = roundLength;
        spawnManager = GetComponent<SpawnManager>();
        StartCoroutine(RoundDisplay(currentRound.ToString(), displayTextFadeOut));
        StartCoroutine(spawnManager.StartSpawning()); // must change when UI is here (button to activate should be easy enough)
    }

    public IEnumerator RoundDisplay(string text, float time)
    {
        displayText.text = text;
        displayText.enabled = true;

        displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, 1);
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
            RoundRestart();
        }
    }

    void RoundRestart()
    {
        StopCoroutine(spawnManager.StartSpawning());
        timeRemaining = roundLength;
        currentRound++;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        StartCoroutine(RoundDisplay(currentRound.ToString(), displayTextFadeOut));
    }
}
