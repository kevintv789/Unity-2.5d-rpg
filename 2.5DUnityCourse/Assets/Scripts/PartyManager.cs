using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField]
    private PartyMemberInfo[] partyMembers;

    [SerializeField]
    private List<PartyMember> currentParty;

    [SerializeField]
    private PartyMemberInfo defaultPartyMember;

    private Vector3 playerPosition;
    private static GameObject instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
            AddMemberToPartyByName(defaultPartyMember.Name);
            AddMemberToPartyByName(defaultPartyMember.Name);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMemberToPartyByName(string name)
    {
        for (int i = 0; i < partyMembers.Length; i++)
        {
            var partyMember = partyMembers[i];
            if (partyMember.Name == name)
            {
                PartyMember newPartyMember = new PartyMember(partyMember);
                currentParty.Add(newPartyMember);
            }
        }
    }

    public List<PartyMember> GetCurrentParty()
    {
        return currentParty.FindAll(member => member.CurrentHealth > 0);
    }

    public void SaveHealth(int partyMemberIndex, int health)
    {
        currentParty[partyMemberIndex].CurrentHealth = health;
    }

    public void SetPosition(Vector3 position)
    {
        playerPosition = position;
    }

    public Vector3 GetPosition()
    {
        return playerPosition;
    }
}

[System.Serializable] // This makes the class visible in the inspector, so we can see CurrentParty
public class PartyMember
{
    public string Name;
    public int Level;
    public int MaxHealth;
    public int CurrentHealth;
    public int Strength;
    public int Initiative;
    public int CurrentExp;
    public int MaxExp;
    public GameObject MemberBattleVisualPrefab; // What will be displayed in battle scene
    public GameObject MemberOverworldVisualPrefab; // What will be displayed in the overworld scene

    public PartyMember(PartyMemberInfo info)
    {
        Name = info.Name;
        Level = info.StartingLevel;
        MaxHealth = info.BaseHealth;
        CurrentHealth = MaxHealth;
        Strength = info.BaseStrength;
        Initiative = info.BaseInitiative;
        CurrentExp = 0;
        MaxExp = 100;
        MemberBattleVisualPrefab = info.MemberBattleVisualPrefab;
        MemberOverworldVisualPrefab = info.MemberOverworldVisualPrefab;
    }
}
