using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    // IN GAME SCENE, OPTIONS SLIDERS ARE MANAGED BY PlayerUIManager SCRIPT

    public Slider soundSlider;
    public Slider musicSlider;
    public Slider sensSlider;

    public static float soundMultiplier = 1f;
    public static float musicMultiplier = 1f;
    public static float sensMultiplier = 1f;


    void Awake()
    {
        // soundSlider.onValueChanged.AddListener();
        // musicSlider.onValueChanged.AddListener();
        sensSlider.onValueChanged.AddListener(SetSens);

        sensSlider.value = sensMultiplier;
    }
    

    public void SetSens(float sens)
    {
        sensMultiplier = sens;
    }
}
