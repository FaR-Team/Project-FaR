using IngameDebugConsole;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour
{
    public static SkillTreeUI Instance;
    public static bool SkillTreeIsOpen = false;

    public GameObject skillTreePanel;
    public TextMeshProUGUI skillPointsText;
    public Button[] skillButtons;
    public TextMeshProUGUI[] skillLevelTexts;
    public SkillData[] skillData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateUI();
        skillTreePanel.SetActive(false);
    }

    private void Update()
    {
        if (GameInput.playerInputActions.Player.SkillTree.WasPressedThisFrame() &&
            !UIController.isPlayerInventoryOpen &&
            !ShopKeeper.Instance.IsBuying &&
            !SleepHandler.Instance._isSleeping &&
            !DebugLogManager.Instance.isOnConsole)
        {
            ToggleSkillTree();
        }
    }

    private void ToggleSkillTree()
    {
        if (SkillTreeIsOpen)
        {
            CloseSkillTree();
        }
        else
        {
            OpenSkillTree();
        }
    }

    public void OpenSkillTree()
    {
        SkillTreeIsOpen = true;
        skillTreePanel.SetActive(true);
        UpdateUI();
        PauseMenu.Instance.Pause();
    }

    public void CloseSkillTree()
    {
        SkillTreeIsOpen = false;
        skillTreePanel.SetActive(false);
        PauseMenu.Instance.Unpause();
    }
    public void UpdateUI()
    {
        skillPointsText.text = "Skill Points: " + PlayerStats.Instance.skillPoints;

        for (int i = 0; i < PlayerStats.Instance.skills.Count; i++)
        {
            Skill skill = PlayerStats.Instance.skills[i];
            SkillData data = skillData[i];
            skillLevelTexts[i].text = data.skillName + " (Level " + skill.level + ")";
            skillButtons[i].interactable = PlayerStats.Instance.skillPoints >= data.costPerLevel[skill.level];
        }
    }
    public void OnSkillButtonClicked(int skillIndex)
    {
        if (PlayerStats.Instance.UpgradeSkill(skillData[skillIndex].type))
        {
            UpdateUI();
        }
    }
}