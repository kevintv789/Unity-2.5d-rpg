using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    public string Name;
    public int StartingLevel;
    public int BaseHealth;
    public int BaseStrength;
    public int BaseInitiative;
    public GameObject MemberBattleVisualPrefab; // What will be displayed in battle scene
    public GameObject MemberOverworldVisualPrefab; // What will be displayed in the overworld scene
}
