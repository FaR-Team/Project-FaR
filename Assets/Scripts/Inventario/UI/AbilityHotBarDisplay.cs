using UnityEngine;

public class AbilityHotBarDisplay : HotbarDisplayBase 

{
    public ToolItemData hoeData, bucketData, blank1Data, blank2Data, blank3Data;
    private int _maxAbilityIndexSize = 5;
    public static int _currentAbilityIndex = 0;

    public static bool isInAbilityHotbarNow;

    public override void Update()
    {
        if (!AbilitiesSlot_UI.isAbilityHotbarActive || 
            !isInAbilityHotbarNow ||
            !HotbarDisplay.CurrentIndexIsSpecialSlotAndYouAreHoldingCtrl()) return;

        if (MouseWheelValue() > 0.1f) 
            ChangeAbilityIndex(-1);

        if (MouseWheelValue() < -0.1f) 
            ChangeAbilityIndex(1);
    }
    public void ChangeAbilityIndex(int newIndex)
    {
        UpdateInventorySlotWithIndex(data, 1, 10);
        DoChangeNameDisplay();
    }

}