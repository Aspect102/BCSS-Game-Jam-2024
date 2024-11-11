using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public PlayerUIManager playerUIManager;
    public PlayerCombat playerCombat;
    public RoundManager roundManager;
    public PlayerController playerController;

    public GameObject player;
    private Card brawlerCard;
    private Card fastHandsCard;
    private Card sprinterCard;
    private Card healerCard;
    public List<Card> playerCards = new List<Card>();
    public List<Card> cards = new List<Card>();

    private void Awake()
    {
        brawlerCard = new Card("brawler", "orange", 15f, 0.1f, 0.05f, -0.1f, 0f, 0f, 0f, 0f, 0f, 0f);
        fastHandsCard = new Card("fastHands", "orange", 15f, -0.05f, 0f, 0.1f, 0f, 0f, 0f, 0f, 0f, 0f);
        sprinterCard = new Card("sprinter", "blue", 10f, 0f, 0f, 0f, 0f, 0f, 0f, -0.05f, 0f, 0.05f);
        healerCard = new Card("medic", "green", 5f, 0f, 0f, 0f, 0.05f, 0f, 0f, 0f, 0f, 0f);

        cards.Add(brawlerCard);
        cards.Add(fastHandsCard);
        cards.Add(sprinterCard);
        cards.Add(healerCard);
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
        playerController.maxDashCooldown *= card.dashCooldown + 1;
        playerController.dashSpeed *= card.dashDistance + 1;
        playerController.playerSpeed *= card.walkSpeed + 1;


        //. . .
        roundManager.RoundRestart();
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
    public float dashCooldown;      // IN PERCENTAGES!!!!!!
    public float dashDistance;      // IN PERCENTAGES!!!!!!
    public float walkSpeed;         // IN PERCENTAGES!!!!!!

    public Card(string cardName, string cardColour, float cardCost, float weaponDamage, float weaponRange, float weaponSwingSpeed, float health, float enemyDamage, float enemyAttackSpeed, float dashCooldown, float dashDistance, float walkSpeed)
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
        this.dashCooldown = dashCooldown;
        this.dashDistance = dashDistance;
        this.walkSpeed = walkSpeed;
    }
}
