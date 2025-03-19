using System;
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

    private void OnEnable()
    {
        GameStateLoader.OnGameStateLoaded += LoadStats;
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

    public void LoadStats(GameStateData gameData)
    {
        skillPoints = gameData.PlayerStatsData.skillPoints;
        hasPants = gameData.PlayerStatsData.hasPants;
        hasShirt = gameData.PlayerStatsData.hasShirt;
        Debug.Log("Loading Stats");
        if (gameData.PlayerStatsData.skillAndLevels != null)
        {
            for (int i = 0; i < gameData.PlayerStatsData.skillAndLevels.Length; i++)
            {
                var skillAndLevel = gameData.PlayerStatsData.skillAndLevels[i];
                Debug.Log("Loading skill " + skillAndLevel.skillType);
                skillLevels[skillAndLevel.skillType] = skillAndLevel.level;
            }
        }
    }
}

[System.Serializable]
public struct PlayerStatsData
{
    public int skillPoints;
    public bool hasPants;
    public bool hasShirt;
    public SkillAndLevel[] skillAndLevels;
    
    public PlayerStatsData(PlayerStats playerStats)
    {
        skillPoints = playerStats.skillPoints;
        hasPants = PlayerStats.hasPants;
        hasShirt = PlayerStats.hasShirt;
        skillAndLevels = new SkillAndLevel[playerStats.skillLevels.Count];
        int i = 0;
        Debug.Log("Creating PlayerStatsData. Unlocked skills count: " + playerStats.skillLevels.Count);
        foreach (var kvp in playerStats.skillLevels)
        {
            skillAndLevels[i] = new SkillAndLevel(kvp.Key, kvp.Value);
            i++;
        }
    }
    
}

[System.Serializable]
public class SkillAndLevel
{
    public SkillType skillType;
    public int level;

    public SkillAndLevel(SkillType skillType, int level)
    {
        this.skillType = skillType;
        this.level = level;
    }
}
public enum SkillType
{
    AreaHarvestSkill,
    QualitySkill,
    AxeSkill,
    HoeSkill,
    ShovelSkill,
    BucketSkill
}