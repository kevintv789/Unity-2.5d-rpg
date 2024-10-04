using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    [SerializeField]
    private Encounter[] enemiesInScene;

    [SerializeField]
    private int maxNumEnemies = 3;

    private EnemyManager enemyManager;

    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        enemyManager.GenerateEnemiesByEncounter(enemiesInScene, maxNumEnemies);
    }
}

[System.Serializable]
public class Encounter
{
    public EnemyInfo Enemy;
    public int LevelMin;
    public int LevelMax;

    public Encounter(EnemyInfo enemy, int levelMin, int levelMax)
    {
        Enemy = enemy;
        LevelMin = levelMin;
        LevelMax = levelMax;
    }
}
