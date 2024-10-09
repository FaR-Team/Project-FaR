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

    [SerializeField] private SkillData[] availableSkills;

    private void Start()
    {
        InitializeSkills();
    }

    private void InitializeSkills()
    {
        foreach (var skillData in availableSkills)
        {
            skills.Add(new Skill { data = skillData, level = 0 });
        }
    }

    public bool UpgradeSkill(SkillType skillType)
    {
        Skill skill = skills.Find(s => s.data.type == skillType);
        if (skill != null && skill.level < skill.data.maxLevel && skillPoints >= skill.data.costPerLevel[skill.level])
        {
            skillPoints -= skill.data.costPerLevel[skill.level];
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
        Skill skill = skills.Find(s => s.data.type == skillType);
        if (skill != null && skill.level < skill.data.maxLevel)
        {
            skill.level++;
            return true;
        }
        return false;
    }

    public int GetSkillLevel(SkillType skillType)
    {
        Skill skill = skills.Find(s => s.data.type == skillType);
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
    public SkillData data;
    public int level;
}