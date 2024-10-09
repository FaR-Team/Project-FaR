using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }
    }

    public List<Skill> skills = new List<Skill>();

    public int skillPoints = 0;

    public static bool hasPants;
    public static bool hasShirt;

    private void Start()
    {
        InitializeSkills();
    }

    private void InitializeSkills()
    {
        skills.Add(new Skill { name = "Area Harvest", 
        type = SkillType.AreaHarvestSkill, level = 0, maxLevel = 2, costPerLevel = new int[] { 1, 2} });
    }

    public bool UpgradeSkill(SkillType skillType)
    {
        Skill skill = skills.Find(s => s.type == skillType);
        if (skill != null && skill.level < skill.maxLevel && skillPoints >= skill.costPerLevel[skill.level])
        {
            skillPoints -= skill.costPerLevel[skill.level];
            skill.level++;
            return true;
        }
        return false;
    }

    public void GiveSkillPoints(int amount)
    {
        skillPoints += amount;
    }

    public bool CheatUpgradeSkill(SkillType skillType)
    {
        Skill skill = skills.Find(s => s.type == skillType);
        if (skill != null && skill.level < skill.maxLevel)
        {
            skill.level++;
            return true;
        }
        return false;
    }

    public int GetSkillLevel(SkillType skillType)
    {
        Skill skill = skills.Find(s => s.type == skillType);
        return skill != null ? skill.level : 0;
    }
}


public enum SkillType
{
    AreaHarvestSkill
}

[System.Serializable]
public class Skill
{
    public string name;
    public SkillType type;
    public int level;
    public int maxLevel;
    public int[] costPerLevel;
}
