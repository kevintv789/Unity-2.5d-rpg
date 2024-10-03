using System.Collections.Generic;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [Header("Battlers")]
    [SerializeField]
    private List<BattleEntities> allBattlers = new List<BattleEntities>();

    [SerializeField]
    private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    [SerializeField]
    private List<BattleEntities> enemyBattlers = new List<BattleEntities>();

    [Header("Spawn Points")]
    [SerializeField]
    private Transform[] partySpawnPoints;

    [SerializeField]
    private Transform[] enemySpawnPoints;

    [Header("UI")]
    [SerializeField]
    private GameObject[] enemySelectionButtons;

    [SerializeField]
    private GameObject battleMenu;

    [SerializeField]
    private GameObject enemySelectionMenu;

    [SerializeField]
    private TextMeshProUGUI actionText;

    [SerializeField]
    private GameObject bottomTextPopup;

    [SerializeField]
    private TextMeshProUGUI bottomText;

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayerIndex;

    private const string ACTION_TEXT_FORMAT = "{0}'s Action:";

    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();

        ShowBattleMenu();

        AttackAction(playerBattlers[0], enemyBattlers[0]);
    }

    private void CreatePartyEntities()
    {
        // Get current party
        List<PartyMember> currentParty = partyManager.GetCurrentParty();

        // Create BattleEntities for each party member
        int i = 0;
        foreach (PartyMember member in currentParty)
        {
            BattleEntities battler = new BattleEntities(
                member.Name,
                member.MaxHealth,
                member.CurrentHealth,
                member.Level,
                member.Initiative,
                member.Strength,
                true
            );

            // Spawn battler at party spawn point
            BattleVisuals tempBattleVisuals = Instantiate(
                    member.MemberBattleVisualPrefab,
                    partySpawnPoints[i].position,
                    Quaternion.identity
                )
                .GetComponent<BattleVisuals>();
            tempBattleVisuals.SetStartingValues(member.MaxHealth, member.MaxHealth, member.Level);
            battler.SetBattleVisuals(tempBattleVisuals);

            playerBattlers.Add(battler);
            allBattlers.Add(battler);
            i++;
        }
    }

    private void CreateEnemyEntities()
    {
        // Get current enemy
        List<Enemy> currentEnemy = enemyManager.GetCurrentEnemies();

        // Create BattleEntities for each enemy
        int i = 0;
        foreach (Enemy enemy in currentEnemy)
        {
            BattleEntities battler = new BattleEntities(
                enemy.Name,
                enemy.MaxHealth,
                enemy.CurrentHealth,
                enemy.Level,
                enemy.Initiative,
                enemy.Strength,
                false
            );

            // Spawn battler at enemy spawn point
            BattleVisuals tempBattleVisuals = Instantiate(
                    enemy.EnemyBattleVisualPrefab,
                    enemySpawnPoints[i].position,
                    Quaternion.identity
                )
                .GetComponent<BattleVisuals>();

            tempBattleVisuals.SetStartingValues(enemy.MaxHealth, enemy.MaxHealth, enemy.Level);
            battler.SetBattleVisuals(tempBattleVisuals);

            enemyBattlers.Add(battler);
            allBattlers.Add(battler);
            i++;
        }
    }

    public void ShowBattleMenu()
    {
        actionText.text = string.Format(
            ACTION_TEXT_FORMAT,
            playerBattlers[currentPlayerIndex].Name
        );

        battleMenu.SetActive(true);
    }

    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        // disable all buttons
        foreach (GameObject button in enemySelectionButtons)
        {
            button.SetActive(false);
        }

        // enable buttons for enemies that are not dead
        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            if (enemyBattlers[i].CurrentHealth > 0)
            {
                enemySelectionButtons[i].SetActive(true);
                enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                    enemyBattlers[i].Name;
            }
        }
    }

    public void SelectEnemy(int curEnemyIndex)
    {
        // Set party member's target to the enemy they selected
        BattleEntities currentPlayer = playerBattlers[currentPlayerIndex];
        currentPlayer.SetTargetIndex(allBattlers.IndexOf(enemyBattlers[curEnemyIndex]));

        currentPlayer.BattleAction = BattleEntities.Action.Attack;
        currentPlayerIndex++;

        // Create a reference for enemy target
        BattleEntities currentTarget = enemyBattlers[curEnemyIndex];

        // If all players have selected
        if (currentPlayerIndex >= playerBattlers.Count)
        {
            // Start battle
            StartBattle();
        }
        else
        {
            // Show the next player's battle menu
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }

    private void StartBattle()
    {
        enemySelectionMenu.SetActive(false);
    }

    private void AttackAction(BattleEntities currentAttacker, BattleEntities currentTarget)
    {
        // Get damage
        int damage = (int)(currentAttacker.Strength * 1.5f);

        // play attack animation
        currentAttacker.BattleVisuals.PlayAttackAnimation();

        // dealing the damage
        currentTarget.CurrentHealth -= damage;
        currentTarget.BattleVisuals.PlayHitAnimation();

        // update the UI
        currentTarget.UpdateUI();

        bottomText.text = string.Format(
            "{0} attacked {1} for {2} damage",
            currentAttacker.Name,
            currentTarget.Name,
            damage
        );
    }
}

[System.Serializable]
public class BattleEntities
{
    public enum Action
    {
        Attack,
        Run,
    };

    public Action BattleAction;

    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int Level;
    public int Initiative;
    public int Strength;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;
    public int TargetIndex;

    public BattleEntities(
        string name,
        int maxHealth,
        int currentHealth,
        int level,
        int initiative,
        int strength,
        bool isPlayer
    )
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Level = level;
        Initiative = initiative;
        Strength = strength;
        IsPlayer = isPlayer;
    }

    public void SetBattleVisuals(BattleVisuals battleVisuals)
    {
        BattleVisuals = battleVisuals;
    }

    public void SetTargetIndex(int targetIndex)
    {
        TargetIndex = targetIndex;
    }

    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurrentHealth);
    }
}
