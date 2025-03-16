using UnityEngine;
[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/SpecialItem/SkillFruit")]
public class SkillFruitItemData : SpecialItemData
{
    public override bool UseItem()
    {
        PlayerStats.Instance.GiveSkillPoints(1);
        return true;
    }
}
