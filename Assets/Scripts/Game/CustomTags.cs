using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTags : MonoBehaviour
{
    public Material colourMaterial;
    public string colourString;

    void Start()
    {
        colourString = colourMaterial.name;
    }

    void Update()
    {
        
    }
}
