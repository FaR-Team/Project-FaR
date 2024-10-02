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
    /* 
     Estos objetos no son necesarios. solo se necesita un objeto mano */
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

        gridGhost = GridGhost.instance;
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

        ChangeAbilityGamepad();

        ChangeObjectInHandModel();
    }

    void ChangeAbility()
    {
        if (MouseWheelValue() > 0.1f) ChangeAbilityIndex(-1);

        if (MouseWheelValue() < -0.1f) ChangeAbilityIndex(1);
    }

    void ChangeAbilityGamepad()
    {
        if (GetPlayerControls().AbilityHotbarDown.WasPerformedThisFrame()) 
            ChangeAbilityIndex(-1);

        if (GetPlayerControls().AbilityHotbarUp.WasPerformedThisFrame()) 
            ChangeAbilityIndex(1);
    }

    private void ChangeAbilityIndex(int direction)
    {
        //This is a void to ask god if he's de boca DE BOKITA
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
        GetPlayerControls().Hotbar.performed += Hotbar;
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
        //_playerControls.Disable();

        GetPlayerControls().Hotbar.performed -= Hotbar;

        GetPlayerControls().UseItem.performed -= UseItem;
        GetPlayerControls().UseItemHoldStart.performed -= x => UseItemPressed();
        GetPlayerControls().UseItemHoldRelease.performed -= x => UseItemRelease();
        GetPlayerControls().Sprint.performed -= x => SellAllPressed();
        GetPlayerControls().SprintFinish.performed -= x => SellAllRelease();
    }

    #region Hotbar Select Methods

    private void Hotbar(InputAction.CallbackContext obj)
    {
        int i = (int)obj.ReadValue<float>() - 1;
        
        if(i == -1) return;
        
        HandleIndex(i);
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
        if (!_isHoldingCtrl || !_isHolding || !interactor.IsLookingAtStore) return; //TODO: && GetPlayerControls().UseItem.WasPressedThisFrame()

        if (GetItemData() == null || !GetItemData().Sellable) return;

        if (GetItemData().IsCropSeed() || !GetItemData().Usable) return;

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
            if (GetItemData().Sellable &&
                !GetItemData().IsCropSeed() &&
                GetItemData().Usable)
            {
                GetItemData().UseItem();
                GetAssignedInventorySlot().SacarDeStack(1);
                GetAssignedInventorySlot().ClearSlot();

                SlotCurrentIndex().UpdateUISlot();
            }
            return;
        }

        if (GetItemData().IsHoe() || GetItemData().IsBucket())
        {
            GetItemData().UseItem();
        }

        if (GetItemData().IsCropSeed() &&
            interactor._LookingAtDirt)
        {
            if (!CanUseItem()) return; // Si la tierra ya tiene algo plantado o no existe

            if (GetItemData().UseItem(dirtToTest))
            {
                GetAssignedInventorySlot().SacarDeStack(1);
                GetAssignedInventorySlot().ClearSlot();
            }
            SlotCurrentIndex().UpdateUISlot();
        }

        if (GetItemData().IsTreeSeed() &&
            gridGhost.CheckCrop(gridGhost.finalPosition, 1) &&
            !interactor._LookingAtDirt)
        {
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


        if (GetItemData().IsHoe())
        {
            hoe.SetActive(true);
            bucket.SetActive(false);
            hand.SetActive(false);
        }
        else if ((!GetItemData().Sellable &&
            !GetItemData().IsCropSeed() &&
            !GetItemData().Usable &&
            !GetItemData().IsTool())

            ||

            GetItemData().Usable)
        {
            hoe.SetActive(false);
            bucket.SetActive(false);
            hand.SetActive(true);
        }

        if (GetItemData().IsBucket())
        {
            hoe.SetActive(false);
            bucket.SetActive(true);
            hand.SetActive(false);
        }
    }
    private void UseItem(InputAction.CallbackContext obj)
    {
        if (GetItemData() == null) return;

        if (GetItemData().IsCropSeed() &&
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

        if (GetItemData() != null && GetItemData().IsTreeSeed() &&
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

    private void HandleIndex(int newIndex)
    {
        if (!UIController.isPlayerInventoryOpen)
        {
            SetIndex(newIndex);
        }
        else if (InventoryUIController.instance.hoveredUISlot != null 
        && InventoryUIController.instance.hoveredUISlot.AssignedInventorySlot.ItemData != null)
        {
            InventoryUIController.instance.hoveredUISlot.ParentDisplay.SwapSlots(InventoryUIController.instance.hoveredUISlot, slots[newIndex]);
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

/*
public class Hand : MonoBehaviour
{
    Action handActions;

    PlayerInput2 inputActions;
    void Awake()
    {
        PlayerInputActionAdder.AddAction(accion);
        inputActions.Player.UseItem.performed += handActions;
    }
    
    void accion()
    {
        handActions = HotbarDisplay.GetItemData().Action();

    }
   


}*/