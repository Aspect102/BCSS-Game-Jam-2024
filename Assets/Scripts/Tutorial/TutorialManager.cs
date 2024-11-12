using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI TutorialText;
    public GameObject enemyPrefab;
    public GameObject player;
    public Material[] materials = new Material[4];
    void Start()
    {
        StartCoroutine(juicyTutorialFunctionThatIMade());
    }

    bool w, a, s, d, space, ctrl, mouse1 = false;
    bool WASDsection = true;
    bool SPACEsection, CTRLsection, MOUSE1section, killsection, cardDisplay = false;

    IEnumerator juicyTutorialFunctionThatIMade()
    {
        while (WASDsection)
        {
            TutorialText.text = "Use WASD to move";
            w = Input.GetKeyDown(KeyCode.W) || w;
            a = Input.GetKeyDown(KeyCode.A) || a;
            s = Input.GetKeyDown(KeyCode.S) || s;
            d = Input.GetKeyDown(KeyCode.D) || d;

            if (w && a && s && d)
            {
                WASDsection = false;
                SPACEsection = true;
            }
            yield return null;
        }
        
        while (SPACEsection)
        {
            TutorialText.text = "Press SPACE to jump";
            space = Input.GetKeyDown(KeyCode.Space);

            if (space)
            {
                SPACEsection = false;
                CTRLsection = true;
            }
            yield return null;
        }

        while (CTRLsection)
        {
            TutorialText.text = "Press LEFT CTRL while in air to perform a ground slam";
            ctrl = Input.GetKeyDown(KeyCode.LeftControl);

            if (ctrl && !player.GetComponent<PlayerController>().isGrounded)
            {
                CTRLsection = false;
                MOUSE1section = true;
            }
            yield return null;
        }
        
        while (MOUSE1section)
        {
            TutorialText.text = "Hold or Press MOUSE 1 to attack enemies";
            mouse1 = Input.GetKeyDown(KeyCode.Mouse0);

            if (mouse1)
            {
                MOUSE1section = false;
                killsection = true;
            }
            yield return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, new Vector3(0, 4.5f, 0), Quaternion.identity);
        SpawnManager.SetPrefabProperties(materials, enemy);

        while (killsection)
        {
            TutorialText.text = "Kill the enemy";
            if (GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyHurt>().currentHealth <= 0)
            {
                var x = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyHurt>();
                x.EnableRagdoll();
                x.transform.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                ParticleSystem.EmissionModule emission = x.transform.gameObject.GetComponentInChildren<ParticleSystem>().emission;
                emission.enabled = true;
                Destroy(x.transform.gameObject, 3f);

                killsection = false;
                TutorialText.text = "Douse yourself in the particle emission of your enemies to heal.";
                yield return new WaitForSeconds(3);
                TutorialText.text = "When you kill an enemy, you will gain points based on its colour.";
                yield return new WaitForSeconds(3);
                TutorialText.text = "Use these colour points after each round to buy ability cards.";
                yield return new WaitForSeconds(3);
                TutorialText.text = "If you don't have enough points then the round will continue and you don't get the card.";
                
                cardDisplay = true;
            }
            yield return null;
        }

        if (cardDisplay)
        {
            GameObject card = GameObject.FindGameObjectWithTag("Card");
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            TutorialText.text = "Take the TUTORIAL card to end the tutorial.";
        }
        yield return null;
    }
   
    public void JustHideTheCardThePlayerDoesntKnow(GameObject card) // referenced in editor by card button press
    {
        card.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 100);
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
    }

    void Update()
    {
        
    }
}
