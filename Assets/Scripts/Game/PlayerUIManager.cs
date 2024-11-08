using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject player;
    PlayerController playerController;

    int maxDashCharges;
    int dashCharges;

    public GameObject parent;
    public Image[] staminaDotsArray;


    // Start is called before the first frame update
    void Start()
    {
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
        dashCharges = playerController.dashCharges;
        
        for (int i = 0; i < maxDashCharges; i++)
        {
            Image image = staminaDotsArray[i];
            float alpha;

            if (dashCharges < (maxDashCharges - i))
            {
                alpha = 0.25f;
            }
            else
            {
                alpha = 1f;
            }

            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }
}
