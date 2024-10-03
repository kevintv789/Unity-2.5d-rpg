using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private EnemyInfo[] enemies;

    [SerializeField]
    private List<Enemy> currentEnemies;

    [SerializeField]
    private EnemyInfo defaultEnemy;

    private void Awake()
    {
        AddEnemyByName("Slime", 1);
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
    public GameObject EnemyBattleVisualPrefab; // What will be displayed in battle scene

    public Enemy(EnemyInfo info, int level)
    {
        float levelModifier = LEVEL_MODIFIER * level;

        Name = info.Name;
        MaxHealth = Mathf.RoundToInt(info.BaseHealth + (info.BaseHealth * levelModifier));
        CurrentHealth = MaxHealth;
        Strength = Mathf.RoundToInt(info.BaseStrength + (info.BaseStrength * levelModifier));
        Initiative = Mathf.RoundToInt(info.BaseInitiative + (info.BaseInitiative * levelModifier));
        EnemyBattleVisualPrefab = info.EnemyBattleVisualPrefab;
    }
}
