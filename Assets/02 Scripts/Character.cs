using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string characterJob;
    [SerializeField] private string characterName;
    [SerializeField] private int level;
    [SerializeField] private string description;

    [SerializeField] private int attackPower;
    [SerializeField] private int healthPoints;
    [SerializeField] private int defense;
    [SerializeField][Range(0, 1)] private float criticalChance;

    public string CharacterJob { get => characterJob; set => characterJob = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Level { get => level; set => level = value; }
    public string Description { get => description; set => description = value; }

    public int AttackPower { get => attackPower; set => attackPower = value; }
    public int HealthPoints { get => healthPoints; set => healthPoints = value; }
    public int Defense { get => defense; set => defense = value; }
    public float CriticalChance { get => criticalChance; set => criticalChance = value; }

    public void SetCharacterData(string job, string name, int lvl, string desc, int atk, int hp, int def, float critChance)
    {
        CharacterJob = job;
        CharacterName = name;
        Level = lvl;
        Description = desc;
        AttackPower = atk;
        HealthPoints = hp;
        Defense = def;
        CriticalChance = critChance;
    }
}
