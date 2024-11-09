using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerUIManager : MonoBehaviour
{
    public GameObject player;
    PlayerController playerController;

    int maxDashCharges;
    int dashCharges;

    public GameObject parent;
    public Image[] staminaDotsArray;

    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public Button mainMenuButton;


    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(MainMenuButtonClicked);

        playerController = player.GetComponent<PlayerController>();
        maxDashCharges = playerController.maxDashCharges;

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

    void MainMenuButtonClicked()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single); //IMPORTANT NOTE: THIS MEANS ONLY ONE SCENE AT A TIME!
    }
}
