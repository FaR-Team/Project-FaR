using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using FaRUtils.FPSController;
using Utils;
public class HotbarDisplay : HotbarDisplayBase
{
    [SerializeField] private Interactor interactor;
    
    private GridGhost gridGhost;

    [Header("Tool GameObjects")]
    public GameObject hoe, bucket, shovel, blank2, blank3, hand;
    [SerializeField] private Animator hoeAnimator, bucketAnimator, shovelAnimator, blank2Animator, blank3Animator; 
    /* 
     Estos objetos no son necesarios. solo se necesita un objeto mano */
    [SerializeField] private ToolItemData[] abilityTools;
    [SerializeField] private Dictionary<ToolItemData, bool> abilityToolsDictionary = new Dictionary<ToolItemData, bool>();

    private int _currentAbilityIndex;

    Dirt dirtToTest;

    private Vector3 previousFinalPosition;
    private int _maxAbilityIndexSize;

    private void Awake()
    {
        _playerControls = GameInput.playerInputActions;
        
        if (player == null)
            player = FaRCharacterController.instance.gameObject;
        if (gridGhost == null) 
            gridGhost = GridGhost.instance;
        if (hoeAnimator == null && hoe != null) 
            hoeAnimator = hoe.GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (gridGhost == null) gridGhost = GridGhost.instance;
        _currentIndex = 0;
        _currentAbilityIndex = 0;
        _maxAbilityIndexSize = abilityTools.Length - 1;
        _maxIndexSize = slots.Length - 1;

        SlotCurrentIndex().ToggleHighlight();
        UpdateAbilitySlot(0);
        ChangeObjectInHandModel();
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
    }

    void ChangeAbility()
    {
        if (MouseWheelValue() > 0.1f)
        {
            ChangeAbilityIndex(-1);
            ChangeObjectInHandModel();
        }

        if (MouseWheelValue() < -0.1f)
        {
            ChangeAbilityIndex(1);
            ChangeObjectInHandModel();
        }
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
        
        for (int attempts = 0; attempts < abilityTools.Length; attempts++)
        {
            _currentAbilityIndex = WrapAbilityIndex(_currentAbilityIndex + direction);
            
            if (IsAbilityUnlocked(_currentAbilityIndex))
            {
                UpdateAbilitySlot(direction);
                DoChangeNameDisplay();
                ChangeObjectInHandModel();
                return;
            }
        }
        
        _currentAbilityIndex = initialIndex;
    }

    private int WrapAbilityIndex(int index)
    {
        if (index > _maxAbilityIndexSize) return 0;
        if (index < 0) return _maxAbilityIndexSize;
        return index;
    }

    private bool IsAbilityUnlocked(int index)
    {
        return InventorySlot_UIAbility.IsAbilityUnlocked(index);
    }

    public void SetGridGhost(GridGhost ghost)
    {
        gridGhost = ghost;
    }


    private void UpdateAbilitySlot(int direction = 0)
    {
        if (abilityTools != null && abilityTools.Length > 0 && _currentAbilityIndex >= 0 && _currentAbilityIndex < abilityTools.Length)
        {
            AbilitySlot().AssignedInventorySlot.UpdateInventorySlot(abilityTools[_currentAbilityIndex], 1); 
        }
        
        var abilitySlotUI = AbilitySlot() as InventorySlot_UIAbility;
        if (abilitySlotUI != null)
        {
            abilitySlotUI.UpdateAbilityCarousel(abilityTools, _currentAbilityIndex, direction);
        }
        else
        {
            AbilitySlot().UpdateUISlot();
        }
    }
    
    private static bool IsInAbilityHotbarNow()
    {
        return _currentIndex == 10;
    }

    private void UseItemPressedCallback(InputAction.CallbackContext ctx) => UseItemPressed();
    private void UseItemReleaseCallback(InputAction.CallbackContext ctx) => UseItemRelease();
    private void SellAllPressedCallback(InputAction.CallbackContext ctx) => SellAllPressed();
    private void SellAllReleaseCallback(InputAction.CallbackContext ctx) => SellAllRelease();

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _playerControls.Enable();
        GetPlayerControls().Hotbar.performed += Hotbar;
        GetPlayerControls().HotbarRight.performed += HotbarRight;
        GetPlayerControls().HotbarLeft.performed += HotbarLeft;
        GetPlayerControls().PrimaryUse.performed += UseItemPrimary;
        GetPlayerControls().Interaction.performed += UseItem;
        GetPlayerControls().UseItemHoldStart.performed += UseItemPressedCallback;
        GetPlayerControls().UseItemHoldRelease.performed += UseItemReleaseCallback;
        GetPlayerControls().MassSell.performed += SellAllPressedCallback;
        GetPlayerControls().MassSellFinish.performed += SellAllReleaseCallback;
    }

    

    protected override void OnDisable()
    {
        base.OnDisable();

        _playerControls.Disable();
        GetPlayerControls().Hotbar.performed -= Hotbar;
        GetPlayerControls().HotbarRight.performed -= HotbarRight;
        GetPlayerControls().HotbarLeft.performed -= HotbarLeft;
        GetPlayerControls().UseItem.performed -= UseItem;
        GetPlayerControls().UseItemHoldStart.performed -= UseItemPressedCallback;
        GetPlayerControls().UseItemHoldRelease.performed -= UseItemReleaseCallback;
        GetPlayerControls().MassSell.performed -= SellAllPressedCallback;
        GetPlayerControls().MassSellFinish.performed -= SellAllReleaseCallback;
        CancelInvoke();
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
        if (IsNotGrabingNorPausedNorConsole())
        {
            ChangeIndex(-1);
            DoChangeNameDisplay();
        }
    }

    public void HotbarRight(InputAction.CallbackContext obj)
    {
        if (IsNotGrabingNorPausedNorConsole())
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

    private bool _hasUsedToolThisPress = false;

    private void UseItemRelease()
    {        
        _isHolding = false;
        _hasUsedToolThisPress = false;
        CancelInvoke();
    }

    private void UseItemPressed()
    {
        _isHolding = true;
        _hasUsedToolThisPress = false;

        InvokeRepeating("Holdear", 0, 0.1f);
    }

    private void SellAllRelease()
    {
        _isHoldingCtrl = false;
    }

    private void SellAllPressed()
    {
        _isHoldingCtrl = true;
        // TODO: Se usa? capaz conviene llamrlo desde aca directo en vez de en holdear
    }

    private void SellAll()
    {
        var slot = GetAssignedInventorySlot();
        var itemData = GetItemData();
        if (slot == null || itemData == null) return;

        GameObject prefab = (itemData as CropItemData)?.CropBoxPrefab;

        while (slot.StackSize > 0)
        {
            if (SellSystem.Instance != null)
            {
                SellSystem.Instance.SellItem(prefab, itemData);
            }
            else
            {
                itemData.UseItem();
            }
            slot.SacarDeStack(1);
        }
        slot.ClearSlot();
        SlotCurrentIndex().UpdateUISlot();
    }

    private void Holdear()
    {
        if (!CanProcessItemUse()) return;

        var itemData = GetItemData();
        if (itemData == null) return;

        if (itemData.SingleUsePerPress)
        {
            if (_hasUsedToolThisPress) return;
            _hasUsedToolThisPress = true;
        }

        var context = new ItemUseContext
        {
            Interactor = interactor,
            DirtToTest = dirtToTest,
            GridGhost = gridGhost,
            HoeAnimator = hoeAnimator,
            IsHolding = _isHolding,
            IsHoldingCtrl = _isHoldingCtrl,
            IsLookingAtStore = interactor != null && interactor.IsLookingAtStore
        };

        ItemUseResult result = itemData.UseItem(context);

        if (result.Success)
        {
            if (result.LockMovementDuration > 0f)
            {
                FaRCharacterController.instance?.LockMovementFor(result.LockMovementDuration);
            }

            if (result.TriggerPlowAnim && hoeAnimator != null)
            {
                hoeAnimator.SetBool("Plow", true);
                StartCoroutine(ResetPlowAnimation());
            }

            if (result.PlaySound)
            {
                PlayItemSound(itemData);
            }

            if (result.ShouldSellSingle)
            {
                SellSingleItem(itemData);
                return;
            }

            if (result.ShouldSellAll)
            {
                SellAll();
                return;
            }

            if (result.ShouldConsume)
            {
                ConsumeItem();
            }

            UpdateCurrentSlot();
        }
    }

    private bool CanProcessItemUse()
    {
        return GetItemData() != null && 
               !interactor.HasInteractable && 
               !GetItemData().leftClickUse;
    }

    private bool CanSellItem(InventoryItemData itemData)
    {
        return itemData.Sellable && 
               !itemData.IsCropSeed() && 
               itemData.Usable;
    }

    private void SellSingleItem(InventoryItemData itemData)
    {
        if (SellSystem.Instance != null && itemData != null)
        {
            GameObject prefab = (itemData as CropItemData)?.CropBoxPrefab;
            SellSystem.Instance.SellItem(prefab, itemData);
        }
        else
        {
            itemData?.UseItem();
        }
        ConsumeItem();
        UpdateCurrentSlot();
    }

    private void ConsumeItem()
    {
        GetAssignedInventorySlot().SacarDeStack(1);
        GetAssignedInventorySlot().ClearSlot();
    }

    private void UpdateCurrentSlot()
    {
        SlotCurrentIndex().UpdateUISlot();
    }

    private void PlayItemSound(InventoryItemData itemData)
    {
        if (itemData.useItemSound != null)
        {
            AudioSource audioSource = player.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(itemData.useItemSound);
            }
        }
    }
    
    private void UseItemPrimary(InputAction.CallbackContext obj)
    {        
        var itemData = GetItemData();
        if (itemData == null || !itemData.leftClickUse) 
        {
            return;
        }

        var context = new ItemUseContext
        {
            Interactor = interactor,
            DirtToTest = dirtToTest,
            GridGhost = gridGhost,
            HoeAnimator = hoeAnimator,
            IsHolding = _isHolding,
            IsHoldingCtrl = _isHoldingCtrl,
            IsLookingAtStore = interactor != null && interactor.IsLookingAtStore
        };

        ItemUseResult result = itemData.UseItem(context);

        if (result.Success)
        {
            if (result.LockMovementDuration > 0f)
            {
                FaRCharacterController.instance?.LockMovementFor(result.LockMovementDuration);
            }

            if (result.TriggerPlowAnim && hoeAnimator != null)
            {
                hoeAnimator.SetBool("Plow", true);
                StartCoroutine(ResetPlowAnimation());
            }

            if (result.PlaySound)
            {
                PlayItemSound(itemData);
            }

            if (result.ShouldConsume)
            {
                ConsumeItem();
            }

            UpdateCurrentSlot();
        }
    }

    private IEnumerator ResetPlowAnimation()
    {
        yield return new WaitForSeconds(1.07f);
        hoeAnimator.SetBool("Plow", false);
    }

    private void ChangeObjectInHandModel()
    {
        SetAllToolsInactive();

        var itemData = GetItemData();
        if (itemData == null)
        {
            hand.SetActive(true);
            return;
        }

        if (itemData.Category == ItemCategory.Tool)
        {
            SetToolModel(itemData);
        }
        else
        {
            hand.SetActive(true);
        }
    }

    private void SetAllToolsInactive()
    {
        if (hoe != null) hoe.SetActive(false);
        if (bucket != null) bucket.SetActive(false);
        if (shovel != null) shovel.SetActive(false);
        if (blank2 != null) blank2.SetActive(false);
        if (blank3 != null) blank3.SetActive(false);
        if (hand != null) hand.SetActive(false);
    }

    private void SetToolModel(InventoryItemData toolData)
    {
        if (toolData == null)
        {
            if (hand != null) hand.SetActive(true);
            return;
        }

        if (toolData.IsHoe() && hoe != null)
        {
            hoe.SetActive(true);
        }
        else if (toolData.IsBucket() && bucket != null)
        {
            bucket.SetActive(true);
        }
        else if (toolData.IsShovel() && shovel != null)
        {
            shovel.SetActive(true);
        }
        else
        {
            if (hand != null) hand.SetActive(true);
        }
    }

    private void UseItem(InputAction.CallbackContext obj)
    {
        // We deleted all of the code cuz it was being done in Holdear
    }

    private void HandleIndex(int newIndex)
    {
        if (!UIController.isPlayerInventoryOpen)
        {
            SetIndex(newIndex);
            ChangeObjectInHandModel();
        }
        else if (InventoryUIController.instance.hoveredUISlot != null 
        && InventoryUIController.instance.hoveredUISlot.AssignedInventorySlot.ItemData != null)
        {
            InventoryUIController.instance.hoveredUISlot.ParentDisplay.SwapSlots(InventoryUIController.instance.hoveredUISlot, slots[newIndex]);
            ChangeObjectInHandModel();
        }
    }

    private void SetIndex(int newIndex)
    {
        SlotCurrentIndex().ToggleHighlight();
        if (newIndex < 0) _currentIndex = 0;
        if (newIndex > _maxIndexSize) newIndex = _maxIndexSize;

        _currentIndex = newIndex;
        _hasUsedToolThisPress = false;
        SlotCurrentIndex().ToggleHighlight();
        DoChangeNameDisplay();
    }

    protected override void ChangeIndex(int direction)
    {
        base.ChangeIndex(direction);
        ChangeObjectInHandModel();
    }
}