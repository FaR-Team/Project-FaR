using UnityEngine;
using TMPro;
using IngameDebugConsole;

public class SkillTreeUI : MonoBehaviour
{
    public static SkillTreeUI Instance;
    public static bool SkillTreeIsOpen = false;

    public GameObject skillTreePanel;
    public TextMeshProUGUI skillPointsText;
    public SkillData[] skillData;

    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color unavailableColor = Color.gray;
    [SerializeField] private Color purchasedColor = Color.green;

    [SerializeField] private SkillNodeButton[] skillNodeButtons;

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
        InitializeSkillNodes();
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

    private void InitializeSkillNodes()
    {
        foreach (var button in skillNodeButtons)
        {
            SkillData data = System.Array.Find(skillData, s => s.type == button.skillType);
            if (data != null && button.nodeIndex < data.costPerLevel.Length)
            {
                button.cost = data.costPerLevel[button.nodeIndex];
            }
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

        foreach (var skillNode in skillNodeButtons)
        {
            skillNode.UpdateVisuals(availableColor, unavailableColor, purchasedColor);
        }
    }
}