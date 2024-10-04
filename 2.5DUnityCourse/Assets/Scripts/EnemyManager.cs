using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private EnemyInfo[] enemies;

    [SerializeField]
    private List<Enemy> currentEnemies;

    [SerializeField]
    private EnemyInfo defaultEnemy;

    private static GameObject instance;

    private void Awake()
    {
        // Make sure there is only one instance of this object
        // This is useful for scenes that have multiple enemy managers
        // This is useful for persisting data across scenes
        if (instance == null)
        {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEnemyByName(string name, int level)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            var enemy = enemies[i];
            if (enemy.Name == name)
            {
                Enemy newEnemy = new Enemy(enemy, level);
                currentEnemies.Add(newEnemy);
            }
        }
    }

    public List<Enemy> GetCurrentEnemies()
    {
        return currentEnemies;
    }

    public void GenerateEnemiesByEncounter(Encounter[] encounters, int maxNumEnemies)
    {
        currentEnemies.Clear();

        int numEnemies = Random.Range(1, maxNumEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            // Get a random encounter
            Encounter encounter = encounters[Random.Range(0, encounters.Length)];
            int level = Random.Range(encounter.LevelMin, encounter.LevelMax);
            AddEnemyByName(encounter.Enemy.Name, level);
        }
    }
}

[System.Serializable] // This makes the class visible in the inspector, so we can see CurrentEnemies
public class Enemy
{
    private const float LEVEL_MODIFIER = 0.5f;

    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int Strength;
    public int Initiative;
    public int Level;
    public GameObject EnemyBattleVisualPrefab; // What will be displayed in battle scene

    public Enemy(EnemyInfo info, int level)
    {
        Level = level;
        float levelModifier = LEVEL_MODIFIER * level;

        Name = info.Name;
        MaxHealth = Mathf.RoundToInt(info.BaseHealth + (info.BaseHealth * levelModifier));
        CurrentHealth = MaxHealth;
        Strength = Mathf.RoundToInt(info.BaseStrength + (info.BaseStrength * levelModifier));
        Initiative = Mathf.RoundToInt(info.BaseInitiative + (info.BaseInitiative * levelModifier));
        EnemyBattleVisualPrefab = info.EnemyBattleVisualPrefab;
    }
}
