public class AbilityHotBarDisplay : HotbarDisplay
{
    public ToolItemData hoeData, bucketData, blank1Data, blank2Data, blank3Data;
    private int _maxAbilityIndexSize = 5;
    public int _currentAbilityIndex = 0;

    protected override void OnEnable()
    {
        _playerControls.Enable();
    }

    void Update()
    {
        if (!AbilitiesSlot_UI.isAbilityHotbarActive) return;

        switch (_currentAbilityIndex)
        {
            case 1:
                GetAssignedInventorySlot().UpdateInventorySlot(hoeData, 1);
                SlotCurrentIndex().UpdateUISlot();
                break;
            case 2:
                GetAssignedInventorySlot().UpdateInventorySlot(bucketData, 1);
                SlotCurrentIndex().UpdateUISlot();
                break;
            case 3:
                GetAssignedInventorySlot().UpdateInventorySlot(blank1Data, 1);
                SlotCurrentIndex().UpdateUISlot();
                break;
            case 4:
                GetAssignedInventorySlot().UpdateInventorySlot(blank2Data, 1);
                SlotCurrentIndex().UpdateUISlot();
                break;
            case 5:
                GetAssignedInventorySlot().UpdateInventorySlot(blank3Data, 1);
                SlotCurrentIndex().UpdateUISlot();
                break;
            default:
                break;
            
        }
    }
}