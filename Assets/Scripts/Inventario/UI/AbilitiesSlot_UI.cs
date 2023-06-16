using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesSlot_UI : InventorySlot_UIBasic
{
    public bool IsHoeUnlocked, IsBucketUnlocked, IsBlank1Unlocked, IsBlank2Unlocked, IsBlank3Unlocked;

    public static bool isAbilityHotbarActive;

    void Start()
    {
        isAbilityHotbarActive = false;
    }

    public override void ToggleHighlight()
    {
        isAbilityHotbarActive = true;
        _slotHighlight.SetActive(!_slotHighlight.activeInHierarchy);
    }
}