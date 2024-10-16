using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillNodeButton : MonoBehaviour
{
    public SkillType skillType;
    public int nodeIndex;
    public SkillNodeButton requiredPreviousNode;
    public int cost;

    private Button button;
    private TextMeshProUGUI levelText;
    private Image buttonImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
    }

    public void UpdateVisuals(Color availableColor, Color unavailableColor, Color purchasedColor)
    {
        int skillLevel = PlayerStats.Instance.GetSkillLevel(skillType);
        levelText.text = $"{name} \nCost: {cost}";
        
        if (skillLevel > nodeIndex)
        {
            buttonImage.color = purchasedColor;
        }
        else if (CanUpgrade())
        {
            buttonImage.color = availableColor;
            button.interactable = true;
        }
        else
        {
            buttonImage.color = unavailableColor;
            button.interactable = false;
        }
    }

    private bool CanUpgrade()
    {
        int skillLevel = PlayerStats.Instance.GetSkillLevel(skillType);
        return PlayerStats.Instance.skillPoints >= cost && 
               skillLevel == nodeIndex &&
               (requiredPreviousNode == null || PlayerStats.Instance.GetSkillLevel(requiredPreviousNode.skillType) > requiredPreviousNode.nodeIndex);
    }

    private void OnClick()
    {
        if (CanUpgrade())
        {
            PlayerStats.Instance.UpgradeSkill(skillType, cost);
            SkillTreeUI.Instance.UpdateUI();
        }
    }
}