using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button PlayButton;
    public Button OptionsButton;
    public Button QuitButton;
    public Button TutorialButton;

    public GameObject optionsPanel;
    bool showingOptions = false;


    void Awake()
    {
        PlayButton.onClick.AddListener(PlayButtonClicked);
        OptionsButton.onClick.AddListener(OptionsButtonClicked);
        QuitButton.onClick.AddListener(QuitCoroutineStarter);
        TutorialButton.onClick.AddListener(TutorialClicked);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void PlayButtonClicked()
    { 
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single); //IMPORTANT NOTE: THIS MEANS ONLY ONE SCENE AT A TIME!
    }


    void OptionsButtonClicked()
    {
        if (showingOptions)
        {
            optionsPanel.SetActive(false);
            showingOptions = false;
        }
        else
        {
            optionsPanel.SetActive(true);
            showingOptions = true;
        }
        
    }

    void TutorialClicked()
    {
        SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Single);
    }

    void QuitCoroutineStarter()
    {
        StartCoroutine(QuitCoroutine());
    }


    IEnumerator QuitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
        // Application.Quit() does not do anything while in playing in the editor, only works in a build
        // Line below simulates quitting the game
        // UnityEditor.EditorApplication.isPlaying = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
