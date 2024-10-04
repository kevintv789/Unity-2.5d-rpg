using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    private enum BattleState
    {
        StartPhase,
        SelectionPhase,
        BattlePhase,
        WonPhase,
        LostPhase,
        RunPhase,
    }

    [Header("Battle State")]
    [SerializeField]
    private BattleState battleState;

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
    private const float TURN_DURATION = 1.5f;

    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();

        ShowBattleMenu();
        DetermineBattleOrder();
    }

    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);
        battleState = BattleState.BattlePhase;
        bottomTextPopup.SetActive(true);

        for (int i = 0; i < allBattlers.Count; i++)
        {
            BattleEntities currentBattler = allBattlers[i];

            switch (currentBattler.BattleAction)
            {
                case BattleEntities.Action.Attack:
                    yield return StartCoroutine(AttackRoutine(currentBattler));
                    break;
                case BattleEntities.Action.Run:
                    // Run action
                    yield return StartCoroutine(RunRoutine());
                    break;
                default:
                    Debug.LogError("No action selected");
                    break;
            }
        }

        if (battleState == BattleState.BattlePhase)
        {
            // repeat the loop
            bottomTextPopup.SetActive(true);
            currentPlayerIndex = 0;
            ShowBattleMenu();
        }

        yield return null;
    }

    private IEnumerator RunRoutine()
    {
        if (battleState == BattleState.BattlePhase)
        {
            int runChance = Random.Range(0, 100);
            if (runChance >= 30)
            {
                bottomText.text = "You ran away!";

                battleState = BattleState.RunPhase;
                yield return new WaitForSeconds(TURN_DURATION);

                allBattlers.Clear();
                playerBattlers.Clear();
                enemyBattlers.Clear();

                // Go back to the overworld scene
                SceneManager.LoadScene("OverworldScene");
                yield break;
            }
            else
            {
                bottomText.text = "You failed to run away!";
                yield return new WaitForSeconds(TURN_DURATION);
            }
        }
    }

    private IEnumerator AttackRoutine(BattleEntities currentBattler)
    {
        // Player's turn
        if (currentBattler.IsPlayer)
        {
            // If the target is a player or is out of bounds, set the target to a random enemy
            if (
                allBattlers[currentBattler.TargetIndex].IsPlayer
                || currentBattler.TargetIndex >= allBattlers.Count
            )
            {
                currentBattler.SetTargetIndex(allBattlers.IndexOf(GetRandomEnemy()));
            }

            BattleEntities currentTarget = allBattlers[currentBattler.TargetIndex];
            AttackAction(currentBattler, currentTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if (currentTarget.CurrentHealth <= 0)
            {
                // Enemy is dead
                bottomText.text = string.Format("{0} has been defeated", currentTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);

                enemyBattlers.Remove(currentTarget);
                allBattlers.Remove(currentTarget);

                if (enemyBattlers.Count == 0)
                {
                    // Player won
                    battleState = BattleState.WonPhase;
                    bottomText.text = "You won!";

                    yield return new WaitForSeconds(TURN_DURATION);

                    // Go back to the overworld scene
                    SceneManager.LoadScene("OverworldScene");
                }
            }
        }
        else
        {
            // Enemy's turn
            BattleEntities currentTarget = GetRandomPartyMember();
            AttackAction(currentBattler, currentTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if (currentTarget.CurrentHealth <= 0)
            {
                // Player is dead
                battleState = BattleState.LostPhase;
                playerBattlers.Remove(currentTarget);
                allBattlers.Remove(currentTarget);

                if (playerBattlers.Count <= 0)
                {
                    bottomText.text = "Game over, LOSER!!";
                    yield return new WaitForSeconds(TURN_DURATION);

                    // Go back to the overworld scene
                    SceneManager.LoadScene("OverworldScene");
                }
            }
        }
    }

    // Get random party member so enemy can attack
    private BattleEntities GetRandomPartyMember()
    {
        return playerBattlers[Random.Range(0, playerBattlers.Count)];
    }

    private BattleEntities GetRandomEnemy()
    {
        return enemyBattlers[Random.Range(0, enemyBattlers.Count)];
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

            tempBattleVisuals.SetStartingValues(
                member.MaxHealth,
                member.CurrentHealth,
                member.Level
            );

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

    public void SelectRun()
    {
        battleState = BattleState.SelectionPhase;

        // Set party member's target to the enemy they selected
        BattleEntities currentPlayer = playerBattlers[currentPlayerIndex];

        currentPlayer.BattleAction = BattleEntities.Action.Run;
        currentPlayerIndex++;

        battleMenu.SetActive(false);

        // If all players have selected
        if (currentPlayerIndex >= playerBattlers.Count)
        {
            // Start battle
            StartCoroutine(BattleRoutine());
        }
        else
        {
            // Show the next player's battle menu
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }

    public void SelectEnemy(int curEnemyIndex)
    {
        // Set party member's target to the enemy they selected
        BattleEntities currentPlayer = playerBattlers[currentPlayerIndex];
        currentPlayer.SetTargetIndex(allBattlers.IndexOf(enemyBattlers[curEnemyIndex]));

        currentPlayer.BattleAction = BattleEntities.Action.Attack;
        currentPlayerIndex++;

        // If all players have selected
        if (currentPlayerIndex >= playerBattlers.Count)
        {
            // Start battle
            StartCoroutine(BattleRoutine());
        }
        else
        {
            // Show the next player's battle menu
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }

    private void AttackAction(BattleEntities currentAttacker, BattleEntities currentTarget)
    {
        // Get damage
        int damage = (int)(currentAttacker.Strength * Random.Range(1.0f, 1.75f));

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

        SavePlayerHealth();
    }

    private void SavePlayerHealth()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            partyManager.SaveHealth(i, playerBattlers[i].CurrentHealth);
        }
    }

    private void DetermineBattleOrder()
    {
        // Sort allBattlers by initiative in ascending order
        allBattlers.Sort(
            (battler1, battler2) => -battler1.Initiative.CompareTo(battler2.Initiative)
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
