using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Jueguito Granjil/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public SkillType type;
    public int maxLevel;
    public int[] costPerLevel;
}
