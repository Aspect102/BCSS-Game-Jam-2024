using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI TutorialText;
    void Start()
    {
        TutorialText.text = "Use WASD to move";
        TutorialText.text = "Press SPACE to jump";
        TutorialText.text = "Hold or Press MOUSE 1 to attack enemies";
        TutorialText.text = "Press CTRL while in air to perform a ground slam";
        TutorialText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
