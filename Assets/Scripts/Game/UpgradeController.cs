using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeController : MonoBehaviour
{
    public PlayerUIManager playerUIManager;
    public PlayerCombat playerCombat;
    public RoundManager roundManager;
    public PlayerController playerController;

    public GameObject player;
    private Card brawlerCard, fastHandsCard, sprinterCard, healerCard, doctorOfDeathCard, tankCard, manipulatorCard, atrainCard, hardModeCard;
    public List<Card> playerCards = new List<Card>();
    public List<Card> cards = new List<Card>();

    private void Awake()
    {
        PlayerCombat.attackDamage = 1;
        RunEnemyController.attackDamage = 15;
        RunEnemyController.attackSpeed = 1.5f;
        RangeEnemyController.minAttackSpeed = 2;
        RangeEnemyController.maxAttackSpeed = 3;

        brawlerCard = new Card("brawler", "orange", 5f, 0.1f, 0.05f, -0.1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,("", 0f));
        fastHandsCard = new Card("fastHands", "orange", 5f, -0.05f, 0f, 0.1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,("", 0f));
        sprinterCard = new Card("sprinter", "blue", 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -0.05f, 0f, 0.05f,("", 0f));
        healerCard = new Card("medic", "green", 5f, 0f, 0f, 0f, 0.05f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,("", 0f));
        doctorOfDeathCard = new Card("doctorofdeath", "green", 10f, 0.20f, 0.15f, 0.20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,("healthRecurringNegative", -0.01f));
        tankCard = new Card("tank", "orange", 10f, 0.1f, -0.2f, -0.2f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, -0.2f, ("", 0f));
        manipulatorCard = new Card("manipulator", "purple", 10f, 0f, 0f, 0f, -0.05f, -0.1f, 0f, 0f, -0.1f, 0f, 0f, 0f, ("", 0f));
        atrainCard = new Card("atrain", "blue", 10f, -0.05f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -0.15f, 0.1f, 0.3f, ("", 0f));
        hardModeCard = new Card("hardmode", "orange", 0f, -0.5f, -0.5f, -0.5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, ("healthRecurringNegative", -0.01f));

        cards.Add(brawlerCard);
        cards.Add(fastHandsCard);
        cards.Add(sprinterCard);
        cards.Add(healerCard);
        cards.Add(doctorOfDeathCard);
        cards.Add(tankCard);
        cards.Add(manipulatorCard);
        cards.Add(atrainCard);
        cards.Add(hardModeCard);
    }

    enum lol
    {
        blue = 0,
        orange = 1,
        purple = 2,
        green = 3
    }

    public void ApplyCardProperties(Card card)
    {
        if (card.cardCost > int.Parse(playerUIManager.colourCounters[(int)Enum.Parse<lol>(card.cardColour)].text))
        {
            roundManager.RoundRestart();
            return;
        }

        playerUIManager.colourCounters[(int)Enum.Parse<lol>(card.cardColour)].text = (float.Parse(playerUIManager.colourCounters[(int)Enum.Parse<lol>(card.cardColour)].text) - card.cardCost).ToString();

        PlayerCombat.attackDamage *= card.weaponDamage + 1;
        playerCombat.attackDistance *= card.weaponRange + 1;
        playerCombat.attackDelay *= card.weaponSwingSpeed + 1;
        playerCombat.playerMaxHealth *= card.health + 1;
        RunEnemyController.attackDamage *= card.enemyDamage + 1;
        RunEnemyController.attackSpeed *= card.enemyAttackSpeed + 1;
        RangeEnemyController.minAttackSpeed *= card.enemyAttackSpeed + 1;
        RangeEnemyController.maxAttackSpeed *= card.enemyAttackSpeed + 1;
        playerController.maxDashCooldown *= card.dashCooldown + 1;
        playerController.dashSpeed *= card.dashDistance + 1;
        playerController.playerSpeed *= card.walkSpeed + 1;

        if (card.uniqueRecurring.Item1 != "")
        {
            StartCoroutine(UpdateRecurringValues(card));
        }

        //. . .
        roundManager.RoundRestart();
    }

    public IEnumerator UpdateRecurringValues(Card card)
    {
        if (card.uniqueRecurring.Item1 == "healthRecurringNegative")
        {
            while (true)
            {
                playerCombat.TakeDamage(1);
                yield return new WaitForSeconds(1);
            }
        }
    }
}

public class Card
{
    public string cardName;
    public string cardColour;
    public float cardCost;
    public float weaponDamage;      // IN PERCENTAGES!!!!!!
    public float weaponRange;       // IN PERCENTAGES!!!!!!
    public float weaponSwingSpeed;  // IN PERCENTAGES!!!!!!
    public float health;            // IN PERCENTAGES!!!!!!
    public float enemyDamage;       // IN PERCENTAGES!!!!!!
    public float enemyAttackSpeed;  // IN PERCENTAGES!!!!!!
    public float enemyWalkSpeed;    // IN PERCENTAGES!!!!!!
    public float rangedAttackSpeed; // IN PERCENTAGES!!!!!!
    public float dashCooldown;      // IN PERCENTAGES!!!!!!
    public float dashDistance;      // IN PERCENTAGES!!!!!!
    public float walkSpeed;         // IN PERCENTAGES!!!!!!

    public (string, float) uniqueRecurring;

    public Card(string cardName, string cardColour, float cardCost, float weaponDamage, float weaponRange, float weaponSwingSpeed, float health, float enemyDamage, float enemyAttackSpeed, float enemyWalkSpeed, float rangedAttackSpeed, float dashCooldown, float dashDistance, float walkSpeed, (string, float) uniqueRecurring)
    {
        this.cardName = cardName;
        this.cardColour = cardColour;
        this.cardCost = cardCost;
        this.weaponDamage = weaponDamage;
        this.weaponRange = weaponRange;
        this.weaponSwingSpeed = weaponSwingSpeed;
        this.health = health;
        this.enemyDamage = enemyDamage;
        this.enemyAttackSpeed = enemyAttackSpeed;
        this.enemyWalkSpeed = enemyWalkSpeed;
        this.rangedAttackSpeed = rangedAttackSpeed;
        this.dashCooldown = dashCooldown;
        this.dashDistance = dashDistance;
        this.walkSpeed = walkSpeed;
        this.uniqueRecurring = uniqueRecurring;
    }
}
