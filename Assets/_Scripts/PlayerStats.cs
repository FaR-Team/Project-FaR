using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public int skillPoints;

    [SerializeField]
    public Dictionary<SkillType, int> skillLevels = new Dictionary<SkillType, int>();

    private void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }
    }

    public static bool hasPants;
    public static bool hasShirt;

    public void GiveSkillPoints(int amount)
    {
        skillPoints += amount;
    }

    public int GetSkillLevel(SkillType skillType)
    {
        if (skillLevels.TryGetValue(skillType, out int level))
        {
            return level;
        }
        return 0;
    }

    public bool UpgradeSkill(SkillType skillType, int cost)
    {
        if (skillPoints < cost)
        {
            return false;
        }

        if (!skillLevels.ContainsKey(skillType))
        {
            skillLevels[skillType] = 0;
        }

        skillLevels[skillType]++;
        skillPoints -= cost;

        return true;
    }
}

public enum SkillType
{
    AreaHarvestSkill
}