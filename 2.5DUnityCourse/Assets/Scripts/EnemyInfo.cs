using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject
{
    public string Name;
    public int BaseHealth;
    public int BaseStrength;
    public int BaseInitiative;
    public GameObject EnemyBattleVisualPrefab; // What will be displayed in battle scene
}
