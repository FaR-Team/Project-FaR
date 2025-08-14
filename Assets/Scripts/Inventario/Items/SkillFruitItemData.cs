using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/SpecialItem/SkillFruit")]
public class SkillFruitItemData : SpecialItemData
{
    [Header("Skill Fruit Specific")]
    [SerializeField] private int skillPointsToGive = 1;
    
    public int SkillPointsToGive => skillPointsToGive;
    
    public override bool UseItem()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.GiveSkillPoints(skillPointsToGive);
            return true;
        }
        return false;
    }
}
