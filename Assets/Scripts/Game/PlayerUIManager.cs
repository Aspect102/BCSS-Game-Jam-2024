using System;
using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PlayerUIManager : MonoBehaviour
{
    // IN MAIN MENU SCENE, OPTIONS SLIDERS ARE MANAGED BY OptionsManager SCRIPT

    private PostProcessVolume volume;
    private Vignette vignette;
    public GameObject player;
    PlayerController playerController;
    public PlayerCombat playerCombat;
    public UpgradeController upgradeController;

    int maxDashCharges;
    int dashCharges;

    public GameObject parent;
    public Image[] staminaDotsArray;

    public GameObject healthDisplay;
    TextMeshProUGUI healthText;

    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public Button mainMenuButton;

    public Slider soundSlider;
    public Slider musicSlider;
    public Slider sensSlider;

    public GameObject gameOverPanel;

    public TextMeshProUGUI[] colourCounters = new TextMeshProUGUI[4]; // blue orange purple green
    // soundMultiplier, musicMultiplier and sensMultiplier are static variables set in OptionsManager script in the main menu scene


    void Awake()
    {
        // soundSlider.onValueChanged.AddListener();
        // musicSlider.onValueChanged.AddListener();
        sensSlider.onValueChanged.AddListener(SetSens);
    }


    // Start is called before the first frame update
    void Start()
    {
        healthText = healthDisplay.GetComponent<TextMeshProUGUI>();
        healthText.text = playerCombat.playerMaxHealth.ToString();

        mainMenuButton.onClick.AddListener(MainMenuButtonClicked);

        playerController = player.GetComponent<PlayerController>();
        maxDashCharges = playerController.maxDashCharges;

        sensSlider.value = OptionsManager.sensMultiplier; // must be below setting playerController for some reason

        InitialiseUI();
    }


    void InitialiseUI()
    {
        staminaDotsArray = new Image[parent.transform.childCount];

        for (int i = 0; i < staminaDotsArray.Length; i++)
        {
            staminaDotsArray[i] = parent.transform.GetChild(i).GetComponent<Image>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        // health
        float health = playerCombat.playerHealth;
        float displayedHealth = float.Parse(healthText.text);

        if (displayedHealth != health)
        {
            if (displayedHealth < health)
            {
                displayedHealth += 1;
            }
            else
            {
                displayedHealth -= 1;
            }
        }

        healthText.text = displayedHealth.ToString();

        // dash ui
        dashCharges = playerController.dashCharges;

        for (int i = 0; i < maxDashCharges; i++)
        {
            Image image = staminaDotsArray[i];
            float alpha;

            if (dashCharges < (maxDashCharges - i))
            {
                alpha = 0.5f;
            }
            else
            {
                alpha = 1f;
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        // options menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    public void GameOverDisplay(int kills, int round)
    {
        gameOverPanel.SetActive(true);
        TextMeshProUGUI roundText = GameObject.FindGameObjectWithTag("RoundText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI killedText = GameObject.FindGameObjectWithTag("KilledText").GetComponent<TextMeshProUGUI>();

        roundText.text = $"Round: {round}";
        killedText.text = $"Kills: {kills}";
    }

    public void SetSens(float sens) // sens set in PlayerController script
    {
        OptionsManager.sensMultiplier = sens;
        playerController.setOptionMultipliers();
    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }


    public void UpdateEnemyKilledIcons(GameObject gameobject)
    {

        switch (gameobject.GetComponent<CustomTags>().colourString)
        {
            case ("blue mat"):
                colourCounters[0].text = (Convert.ToInt32(colourCounters[0].text) + 1).ToString();
                break;
            case ("orange mat"):
                colourCounters[1].text = (Convert.ToInt32(colourCounters[1].text) + 1).ToString();
                break;
            case ("purple mat"):
                colourCounters[2].text = (Convert.ToInt32(colourCounters[2].text) + 1).ToString(); // in future maybe change
                break;
            case ("green mat"):
                colourCounters[3].text = (Convert.ToInt32(colourCounters[3].text) + 1).ToString();
                break;
            default:
                Debug.Log("fart alert");
                break;
        }
    }


    void MainMenuButtonClicked()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single); //IMPORTANT NOTE: THIS MEANS ONLY ONE SCENE AT A TIME!
    }


    public void ShowCards()
    {
        GameObject[] cardArray = GameObject.FindGameObjectsWithTag("Card");
        var random1 = Random.Range(0, cardArray.Length);
        var random2 = Random.Range(0, cardArray.Length);
        while (random1 == random2)
        {
            random2 = Random.Range(0, cardArray.Length);
        }
        GameObject[] randomCards = { cardArray[random1], cardArray[random2] };
        randomCards[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-280, 125);
        randomCards[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(320, 125);
        randomCards[0].SetActive(true);
        randomCards[1].SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true; 
    }



    public void CardClicked(GameObject cardObj)
    {
        upgradeController.ApplyCardProperties(upgradeController.cards.Find(c => c.cardName == cardObj.name));
        HideCards();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void HideCards()
    {
        GameObject[] cardArray = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in cardArray) 
        {
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 100); // far away
        }
    }


    public void VignetteCheck(float playerHealth, float maxHealth)
    {
        float[] healthStages = { maxHealth / 5, 2 * maxHealth / 5, 3 * maxHealth / 5, 4 * maxHealth / 5, maxHealth};
        volume = GameObject.FindGameObjectWithTag("VignetteControl").GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<Vignette>(out vignette);
        float[] vignetteStages = { 0f, 0.125f, 0.25f, 0.375f, 0.5f };

        if (playerHealth <= healthStages[0])
        {
            vignette.intensity.Override(vignetteStages[4]);
        }
        else if (playerHealth <= healthStages[1])
        {
            vignette.intensity.Override(vignetteStages[3]);
        }
        else if (playerHealth <= healthStages[2])
        {
            vignette.intensity.Override(vignetteStages[2]);
        }
        else if (playerHealth <= healthStages[3])
        {
            vignette.intensity.Override(vignetteStages[1]);
        }
        else if (playerHealth <= healthStages[4])
        {
            vignette.intensity.Override(vignetteStages[0]);
        }
        else if (playerHealth <= healthStages[5])
        {
            vignette.intensity.Override(vignetteStages[0]);
        }
    }
}
