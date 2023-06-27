using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
public class HotbarDisplay : HotbarDisplayBase
{
    [SerializeField] private Interactor interactor;

    public GameObject telequinesis;
    public GridGhost gridGhost;

    [Header("Tool GameObjects")]
    public GameObject hoe, bucket, blank1, blank2, blank3, hand;
    [SerializeField] private ToolItemData[] abilityTools;
    [SerializeField] private Dictionary<ToolItemData, bool> abilityToolsDictionary = new Dictionary<ToolItemData, bool>();

    private int _currentAbilityIndex;

    private InventoryItemData gameobj1, gameobj2, gameobj3, gameibj4, gameobj5;

    Dirt dirtToTest;

    private Vector3 previousFinalPosition;
    private int _maxAbilityIndexSize;

    private void Awake()
    {
        _playerControls = GameInput.playerInputActions;

        if (telequinesis == null) telequinesis = GameObject.FindWithTag("Telequinesis");
        if (player == null) player = GameObject.FindWithTag("Player");

    }

    protected override void Start()
    {
        base.Start();

        _currentIndex = 0;
        _currentAbilityIndex = 0;
        _maxAbilityIndexSize = 1;
        //UpdateAbilitySlot();
        _maxIndexSize = slots.Length - 1;

        SlotCurrentIndex().ToggleHighlight();
    }

    public override void Update()
    {
        base.Update();

        if (InventorySlot_UIAbility.isAbilityHotbarActive &&
            IsInAbilityHotbarNow() &&
            CurrentIndexIsSpecialSlotAndYouAreHoldingCtrl())
        {
            ChangeAbility();
        }

        ChangeObjectInHandModel();
    }

    void ChangeAbility()
    {
        if (MouseWheelValue() > 0.1f) ChangeAbilityIndex(-1);

        if (MouseWheelValue() < -0.1f) ChangeAbilityIndex(1);
    }

    private void ChangeAbilityIndex(int direction)
    {
        //This is a void to ask god if he's de boca
        var initialIndex = _currentAbilityIndex;
        _currentAbilityIndex += direction;
        
        if (_currentAbilityIndex > _maxAbilityIndexSize) _currentAbilityIndex = 0;
        if (_currentAbilityIndex < 0) _currentAbilityIndex = _maxAbilityIndexSize;

        for(int i = 0; i < abilityTools.Length; i++)
        {
            if(InventorySlot_UIAbility.isUnlocked[_currentAbilityIndex] == false)
            {
                _currentAbilityIndex += direction;
        
                if (_currentAbilityIndex > _maxAbilityIndexSize) _currentAbilityIndex = 0;
                if (_currentAbilityIndex < 0) _currentAbilityIndex = _maxAbilityIndexSize;
            }
            else{
                UpdateAbilitySlot();
                return;
            }
        }

        _currentAbilityIndex = initialIndex;
    }

    private void UpdateAbilitySlot()
    {
        AbilitySlot().AssignedInventorySlot.UpdateInventorySlot(abilityTools[_currentAbilityIndex], 1); 
        AbilitySlot().UpdateUISlot();
    }
    
    private static bool IsInAbilityHotbarNow()
    {
        return _currentIndex == 10;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _playerControls.Enable();

        GetPlayerControls().Hotbar1.performed += Hotbar1;
        GetPlayerControls().Hotbar2.performed += Hotbar2;
        GetPlayerControls().Hotbar3.performed += Hotbar3;
        GetPlayerControls().Hotbar4.performed += Hotbar4;
        GetPlayerControls().Hotbar5.performed += Hotbar5;
        GetPlayerControls().Hotbar6.performed += Hotbar6;
        GetPlayerControls().Hotbar7.performed += Hotbar7;
        GetPlayerControls().Hotbar8.performed += Hotbar8;
        GetPlayerControls().Hotbar9.performed += Hotbar9;
        GetPlayerControls().Hotbar10.performed += Hotbar10;
        GetPlayerControls().HotbarRight.performed += HotbarRight;
        GetPlayerControls().HotbarLeft.performed += HotbarLeft;

        GetPlayerControls().UseItem.performed += UseItem;
        GetPlayerControls().UseItemHoldStart.performed += x => UseItemPressed();
        GetPlayerControls().UseItemHoldRelease.performed += x => UseItemRelease();
        GetPlayerControls().MassSell.performed += x => SellAllPressed();
        GetPlayerControls().MassSellFinish.performed += x => SellAllRelease();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _playerControls.Disable();
        GetPlayerControls().Hotbar1.performed -= Hotbar1;
        GetPlayerControls().Hotbar2.performed -= Hotbar2;
        GetPlayerControls().Hotbar3.performed -= Hotbar3;
        GetPlayerControls().Hotbar4.performed -= Hotbar4;
        GetPlayerControls().Hotbar5.performed -= Hotbar5;
        GetPlayerControls().Hotbar6.performed -= Hotbar6;
        GetPlayerControls().Hotbar7.performed -= Hotbar7;
        GetPlayerControls().Hotbar8.performed -= Hotbar8;
        GetPlayerControls().Hotbar9.performed -= Hotbar9;
        GetPlayerControls().Hotbar10.performed -= Hotbar10;
        GetPlayerControls().UseItem.performed -= UseItem;
        GetPlayerControls().UseItemHoldStart.performed -= x => UseItemPressed();
        GetPlayerControls().UseItemHoldRelease.performed -= x => UseItemRelease();
        GetPlayerControls().Sprint.performed -= x => SellAllPressed();
        GetPlayerControls().SprintFinish.performed -= x => SellAllRelease();
    }

    #region Hotbar Select Methods
    private void Hotbar1(InputAction.CallbackContext obj)
    {
        SetIndex(0);
    }

    private void Hotbar2(InputAction.CallbackContext obj)
    {
        SetIndex(1);
    }

    private void Hotbar3(InputAction.CallbackContext obj)
    {
        SetIndex(2);
    }

    private void Hotbar4(InputAction.CallbackContext obj)
    {
        SetIndex(3);
    }

    private void Hotbar5(InputAction.CallbackContext obj)
    {
        SetIndex(4);
    }

    private void Hotbar6(InputAction.CallbackContext obj)
    {
        SetIndex(5);
    }

    private void Hotbar7(InputAction.CallbackContext obj)
    {
        SetIndex(6);
    }

    private void Hotbar8(InputAction.CallbackContext obj)
    {
        SetIndex(7);
    }

    private void Hotbar9(InputAction.CallbackContext obj)
    {
        SetIndex(8);
    }

    private void Hotbar10(InputAction.CallbackContext obj)
    {
        SetIndex(9);
    }
    #endregion

    public void HotbarLeft(InputAction.CallbackContext obj)
    {
        if (GetPlayerControls().HotbarLeft.WasPressedThisFrame() &&
                    IsNotGrabingNorPausedNorConsole())
        {
            ChangeIndex(-1);
            DoChangeNameDisplay();
        }
    }

    public void HotbarRight(InputAction.CallbackContext obj)
    {
        if (GetPlayerControls().HotbarRight.WasPressedThisFrame() &&
                    IsNotGrabingNorPausedNorConsole())
        {
            ChangeIndex(1);
            DoChangeNameDisplay();
        }
    }
    public bool CanUseItem()
    {
        if (gridGhost.finalPosition != previousFinalPosition)
        {
            previousFinalPosition = gridGhost.finalPosition;
            dirtToTest = gridGhost.CheckDirt(gridGhost.finalPosition, 0.2f);
        }

        if (dirtToTest == null)
        {
            return false;
        }

        if (!dirtToTest.IsEmpty)
        {
            return false;
        }

        return true;
    }
    private void UseItemRelease()
    {
        _isHolding = false;
        CancelInvoke();
    }

    private void UseItemPressed()
    {
        _isHolding = true;
        InvokeRepeating("Holdear", 0, 0.1f);
    }

    private void SellAllRelease()
    {
        _isHoldingCtrl = false;
    }

    private void SellAllPressed()
    {
        _isHoldingCtrl = true;
        //SellAll();
    }

    private void SellAll()
    {
        if (!_isHoldingCtrl || !_isHolding || !interactor.IsLookingAtStore) return;//TODO: && GetPlayerControls().UseItem.WasPressedThisFrame()

        if (GetItemData() == null || GetItemData().Sellable == false) return;

        if (GetItemData().Seed != false || GetItemData().Usable != true) return;

        int i = 0;

        while (i < GetAssignedInventorySlot().StackSize)
        {
            GetItemData().UseItem();
            GetAssignedInventorySlot().SacarDeStack(1);
        }
        GetAssignedInventorySlot().ClearSlot();

        SlotCurrentIndex().UpdateUISlot();
    }

    private void Holdear()
    {
        if (GetItemData() == null) return;

        if (_isHolding && !_isHoldingCtrl && interactor.IsLookingAtStore)
        {
            if (GetItemData().Sellable == true &&
                GetItemData().Seed == false &&
                GetItemData().Usable == true)
            {
                // var inventory = player.GetComponent<InventoryHolder>();

                GetItemData().UseItem();
                GetAssignedInventorySlot().SacarDeStack(1);
                GetAssignedInventorySlot().ClearSlot();

                SlotCurrentIndex().UpdateUISlot();
            }
            return;
        }

        if (GetItemData().IsHoe || GetItemData().IsBucket)
        {
            GetItemData().UseItem();
        }

        if (GetItemData().Seed &&
            interactor._LookingAtDirt)
        {
            var dirt = gridGhost.CheckDirt(gridGhost.finalPosition, 0.1f);

            if (!CanUseItem()) return; // Si la tierra ya tiene algo plantado o no existe


            //var inventory = player.GetComponent<InventoryHolder>();

            if (GetItemData().UseItem(dirtToTest) == true)
            {
                GetAssignedInventorySlot().SacarDeStack(1);
                GetAssignedInventorySlot().ClearSlot();
            }
            SlotCurrentIndex().UpdateUISlot();
        }

        if (GetItemData().TreeSeed &&
            gridGhost.CheckCrop(gridGhost.finalPosition, 1) &&
            interactor._LookingAtDirt == false)
        {
            //var inventory = player.GetComponent<InventoryHolder>();

            if (GetItemData().UseItem())
            {
                GetAssignedInventorySlot().SacarDeStack(1);
                GetAssignedInventorySlot().ClearSlot();
            }
            SlotCurrentIndex().UpdateUISlot();
        }
    }

    private void ChangeObjectInHandModel()
    {
        if (GetItemData() == null)
        {
            hoe.SetActive(false);
            bucket.SetActive(false);
            hand.SetActive(true);
            return;
        }


        if (GetItemData().IsHoe)
        {
            hoe.SetActive(true);
            bucket.SetActive(false);
            hand.SetActive(false);
        }
        else if ((!GetItemData().Sellable &&
            !GetItemData().Seed &&
            !GetItemData().Usable &&
            !GetItemData().Tool)

            ||

            GetItemData().Usable)
        {
            hoe.SetActive(false);
            bucket.SetActive(false);
            hand.SetActive(true);
        }

        if (GetItemData().IsBucket)
        {
            hoe.SetActive(false);
            bucket.SetActive(true);
            hand.SetActive(false);
        }
    }
    private void UseItem(InputAction.CallbackContext obj)
    {
        if (GetItemData() == null) return;

        if (GetItemData().Seed &&
            interactor._LookingAtDirt)
        {
            var dirt = gridGhost.CheckDirt(gridGhost.finalPosition, 0.1f);

            if (!CanUseItem()) return; // Si la tierra ya tiene algo plantado o no existe


            //var inventory = player.GetComponent<InventoryHolder>();

            if (GetItemData().UseItem(dirtToTest) == true)
            {
                GetAssignedInventorySlot().SacarDeStack(1);
                GetAssignedInventorySlot().ClearSlot();
            }
            SlotCurrentIndex().UpdateUISlot();
        }

        if (GetItemData().TreeSeed &&
            gridGhost.CheckCrop(gridGhost.finalPosition, 1) &&
            interactor._LookingAtDirt == false)
        {
            //var inventory = player.GetComponent<InventoryHolder>();

            if (GetItemData().UseItem())
            {
                GetAssignedInventorySlot().SacarDeStack(1);
                GetAssignedInventorySlot().ClearSlot();
            }
            SlotCurrentIndex().UpdateUISlot();
        }
    }

    private void SetIndex(int newIndex)
    {
        SlotCurrentIndex().ToggleHighlight();
        if (newIndex < 0) _currentIndex = 0;
        if (newIndex > _maxIndexSize) newIndex = _maxIndexSize;

        _currentIndex = newIndex;
        SlotCurrentIndex().ToggleHighlight();
        DoChangeNameDisplay();
    }
}
