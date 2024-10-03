using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    private List<BattleEntities> allBattlers = new List<BattleEntities>();

    [SerializeField]
    private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    [SerializeField]
    private List<BattleEntities> enemyBattlers = new List<BattleEntities>();

    private PartyManager partyManager;
    private EnemyManager enemyManager;

    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
    }

    private void CreatePartyEntities()
    {
        // Get current party
        List<PartyMember> currentParty = partyManager.GetCurrentParty();

        // Create BattleEntities for each party member
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
            playerBattlers.Add(battler);
            allBattlers.Add(battler);
        }
    }

    private void CreateEnemyEntities()
    {
        // Get current enemy
        List<Enemy> currentEnemy = enemyManager.GetCurrentEnemies();

        // Create BattleEntities for each enemy
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
            enemyBattlers.Add(battler);
            allBattlers.Add(battler);
        }
    }
}

[System.Serializable]
public class BattleEntities
{
    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int Level;
    public int Initiative;
    public int Strength;
    public bool IsPlayer;

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
}
