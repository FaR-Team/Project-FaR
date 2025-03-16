using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InventorySlot_UIAbility : InventorySlot_UIBasic
{
    public bool TESTING;
    public static bool IsHoeUnlocked, IsBucketUnlocked, IsShovelUnlocked, IsBlank2Unlocked, IsBlank3Unlocked;

    [ShowInInspector] public static bool[] isUnlocked = {IsHoeUnlocked, IsBucketUnlocked, IsShovelUnlocked, IsBlank2Unlocked, IsBlank3Unlocked};

    public static bool isAbilityHotbarActive;

    void Start()
    {
        if (TESTING)
        {
            IsHoeUnlocked = true;
            IsBucketUnlocked = true;
            IsShovelUnlocked = true;

            isUnlocked = new bool[] {IsHoeUnlocked, IsBucketUnlocked, IsShovelUnlocked, IsBlank2Unlocked, IsBlank3Unlocked};
        }
        isAbilityHotbarActive = false;
    }

    public override void ToggleHighlight()
    {
        isAbilityHotbarActive = true;
        _slotHighlight.SetActive(!_slotHighlight.activeInHierarchy);
    }
}