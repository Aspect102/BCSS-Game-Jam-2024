using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button PlayButton;
    public Button OptionsButton;
    public Button QuitButton;
    void Awake()
    {
        PlayButton.onClick.AddListener(PlayButtonClicked);
        //OptionsButton.onClick.AddListener();
        //QuitButton.onClick.AddListener();
    }

    void PlayButtonClicked()
    { 
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single); //IMPORTANT NOTE: THIS MEANS ONLY ONE SCENE AT A TIME!
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
