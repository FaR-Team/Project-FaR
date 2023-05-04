using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class HotbarDisplay : StaticInventoryDisplay
{
    private int _maxIndexSize = 9;
    public int _currentIndex = 0;

    [SerializeField] private Interactor interactor;

    public GameObject telequinesis;
    public TextMeshProUGUI NameDisplay;
    public PhysicsGunInteractionBehavior physicsGun;
    public GridGhost gridGhost;
    public GameObject player;

    public PlayerInventoryHolder PlayerInv;

    [Header("Tool GameObjects")]
    public GameObject hoe, bucket, rexona, blank1, blank2, hand;

    private InventoryItemData gameobj1, gameobj2, gameobj3, gameibj4, gameobj5;

    public PlayerInput2 _playerControls;

    private bool _isHolding = false;
    private bool _isHoldingCtrl = false;


    private void Awake()
    {
        _playerControls = new PlayerInput2();
        telequinesis = GameObject.FindWithTag("Telequinesis");
        player = GameObject.FindWithTag("Player");
        PlayerInv = player.GetComponent<PlayerInventoryHolder>();

        physicsGun = telequinesis.GetComponent<PhysicsGunInteractionBehavior>();
    }

    protected override void Start()
    {
        base.Start();

        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;

        slots[_currentIndex].ToggleHighlight();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _playerControls.Enable();

        _playerControls.Player.Hotbar1.performed += Hotbar1;
        _playerControls.Player.Hotbar2.performed += Hotbar2;
        _playerControls.Player.Hotbar3.performed += Hotbar3;
        _playerControls.Player.Hotbar4.performed += Hotbar4;
        _playerControls.Player.Hotbar5.performed += Hotbar5;
        _playerControls.Player.Hotbar6.performed += Hotbar6;
        _playerControls.Player.Hotbar7.performed += Hotbar7;
        _playerControls.Player.Hotbar8.performed += Hotbar8;
        _playerControls.Player.Hotbar9.performed += Hotbar9;
        _playerControls.Player.Hotbar10.performed += Hotbar10;
        _playerControls.Player.UseItem.performed += UseItem;
        _playerControls.Player.UseItemHoldStart.performed += x => UseItemPressed();
        _playerControls.Player.UseItemHoldRelease.performed += x => UseItemRelease();
        _playerControls.Player.MassSell.performed += x => SellAllPressed();
        _playerControls.Player.MassSellFinish.performed += x => SellAllRelease();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _playerControls.Disable();

        _playerControls.Player.Hotbar1.performed -= Hotbar1;
        _playerControls.Player.Hotbar2.performed -= Hotbar2;
        _playerControls.Player.Hotbar3.performed -= Hotbar3;
        _playerControls.Player.Hotbar4.performed -= Hotbar4;
        _playerControls.Player.Hotbar5.performed -= Hotbar5;
        _playerControls.Player.Hotbar6.performed -= Hotbar6;
        _playerControls.Player.Hotbar7.performed -= Hotbar7;
        _playerControls.Player.Hotbar8.performed -= Hotbar8;
        _playerControls.Player.Hotbar9.performed -= Hotbar9;
        _playerControls.Player.Hotbar10.performed -= Hotbar10;
        _playerControls.Player.UseItem.performed -= UseItem;
        _playerControls.Player.UseItemHoldStart.performed -= x => UseItemPressed();
        _playerControls.Player.UseItemHoldRelease.performed -= x => UseItemRelease();
        _playerControls.Player.Sprint.performed -= x => SellAllPressed();
        _playerControls.Player.SprintFinish.performed -= x => SellAllRelease();
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

    private void UseItemRelease()
    {
        _isHolding = false;
        CancelInvoke();
    }

    private void UseItemPressed()
    {
        _isHolding = true;
        InvokeRepeating("Holdear", 0, 0.15f);
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
        if (_isHoldingCtrl && _isHolding && interactor.IsLookingAtStore) //TODO: && _playerControls.Player.UseItem.WasPressedThisFrame()
        {
            if (slots[_currentIndex].AssignedInventorySlot.ItemData != null &&
                slots[_currentIndex].AssignedInventorySlot.ItemData.Sellable == true &&
                slots[_currentIndex].AssignedInventorySlot.ItemData.Seed == false &&
                slots[_currentIndex].AssignedInventorySlot.ItemData.Usable == true)
            {
                var inventory = player.GetComponent<InventoryHolder>();
                int i = 0;
                while (i < slots[_currentIndex].AssignedInventorySlot.StackSize)
                {
                    slots[_currentIndex].AssignedInventorySlot.ItemData.UseItem();
                    slots[_currentIndex].AssignedInventorySlot.SacarDeStack(1);
                }   
                if (slots[_currentIndex].AssignedInventorySlot.StackSize == 0)
                {
                    slots[_currentIndex].AssignedInventorySlot.ClearSlot();
                }
                slots[_currentIndex].UpdateUISlot();
            }
        }
    }
    private void Update()
    {
        if (_playerControls.Player.MouseWheel.ReadValue<float>() > 0.1f && physicsGun._grabbedRigidbody == null && PauseMenu.GameIsPaused == false && PlayerInv.IsBuying == false && IngameDebugConsole.DebugLogManager.Instance.isOnConsole == false)
        {
            ChangeIndex(-1);
            if (slots[_currentIndex].AssignedInventorySlot.ItemData != null)
            {
                NameDisplay.text = slots[_currentIndex].AssignedInventorySlot.ItemData.Nombre;
                if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo == false)
                {
                    NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
                    NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo = true;
                    NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                    NameDisplay.GetComponent<NameDisplayController>()._yaAnimo = false;
                    StartCoroutine(NameDisplay.GetComponent<NameDisplayController>().waiter());
                }
                else if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo == true)
                {
                    NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                }
            }
        }

        if (_playerControls.Player.MouseWheel.ReadValue<float>() < -0.1f && physicsGun._grabbedRigidbody == null && PauseMenu.GameIsPaused == false && PlayerInv.IsBuying == false && IngameDebugConsole.DebugLogManager.Instance.isOnConsole == false)
        {
            ChangeIndex(1);
            if (slots[_currentIndex].AssignedInventorySlot.ItemData != null)
            {
                NameDisplay.text = slots[_currentIndex].AssignedInventorySlot.ItemData.Nombre;
                if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo == false)
                {
                    NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
                    StartCoroutine(NameDisplay.GetComponent<NameDisplayController>().waiter());
                    NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo = true;
                    NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                    NameDisplay.GetComponent<NameDisplayController>()._yaAnimo = false;
                }
                else if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo == true)
                {
                    NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                }
            }
        }

        if (slots[_currentIndex].AssignedInventorySlot.ItemData != null && slots[_currentIndex].AssignedInventorySlot.ItemData.IsHoe == true)
        {
            hand.SetActive(false);
            hoe.SetActive(true);
        }
        else if ((slots[_currentIndex].AssignedInventorySlot.ItemData != null && slots[_currentIndex].AssignedInventorySlot.ItemData.Sellable == false && slots[_currentIndex].AssignedInventorySlot.ItemData.Seed == false && slots[_currentIndex].AssignedInventorySlot.ItemData.Usable == false && slots[_currentIndex].AssignedInventorySlot.ItemData.Tool == false) || slots[_currentIndex].AssignedInventorySlot.ItemData == null || slots[_currentIndex].AssignedInventorySlot.ItemData != null && slots[_currentIndex].AssignedInventorySlot.ItemData.Usable == true)
        {
            hoe.SetActive(false);
            hand.SetActive(true);
        }

        if (_isHoldingCtrl)
        {
            SellAll();
        }
    }
    /*private void FixedUpdate()
    {
        if (_isHoldingCtrl && _isHolding && slots[_currentIndex].AssignedInventorySlot.ItemData.IsLookingAtStore)
        {
            if (slots[_currentIndex].AssignedInventorySlot.ItemData != null &&
                slots[_currentIndex].AssignedInventorySlot.ItemData.Sellable == true &&
                slots[_currentIndex].AssignedInventorySlot.ItemData.Seed == false &&
                slots[_currentIndex].AssignedInventorySlot.ItemData.Usable == true)
            {
                var inventory = player.GetComponent<InventoryHolder>();

                slots[_currentIndex].AssignedInventorySlot.ItemData.UseItem();
                slots[_currentIndex].AssignedInventorySlot.SacarDeStack(slots[_currentIndex].AssignedInventorySlot.StackSize);
                if (slots[_currentIndex].AssignedInventorySlot.StackSize == 0)
                {
                    slots[_currentIndex].AssignedInventorySlot.ClearSlot();
                }
                slots[_currentIndex].UpdateUISlot();
            }
            Debug.Log(Time.deltaTime);
        }
    }*/

    private void Holdear()
    {
        if (_isHolding && !_isHoldingCtrl && interactor.IsLookingAtStore)
        {
            if (slots[_currentIndex].AssignedInventorySlot.ItemData != null && 
                slots[_currentIndex].AssignedInventorySlot.ItemData.Sellable == true && 
                slots[_currentIndex].AssignedInventorySlot.ItemData.Seed == false && 
                slots[_currentIndex].AssignedInventorySlot.ItemData.Usable == true)
            {
                var inventory = player.GetComponent<InventoryHolder>();

                slots[_currentIndex].AssignedInventorySlot.ItemData.UseItem();
                slots[_currentIndex].AssignedInventorySlot.SacarDeStack(1);
                if (slots[_currentIndex].AssignedInventorySlot.StackSize == 0)
                {
                    slots[_currentIndex].AssignedInventorySlot.ClearSlot();
                }
                slots[_currentIndex].UpdateUISlot();
            }
        }
    }

    private void UseItem(InputAction.CallbackContext obj)
    {
        var slot_CurrentIndex_ItemData = slots[_currentIndex].AssignedInventorySlot.ItemData;

        if (slot_CurrentIndex_ItemData != null &&
            slot_CurrentIndex_ItemData.Usable == true &&
            slot_CurrentIndex_ItemData.Seed == false &&
            slot_CurrentIndex_ItemData.Sellable == false &&
            slot_CurrentIndex_ItemData.TreeSeed == false)

        { slot_CurrentIndex_ItemData.UseItem(); }

        /*if (slot_CurrentIndex_ItemData != null &&
            slot_CurrentIndex_ItemData.Sellable == true &&
            slot_CurrentIndex_ItemData.Seed == false &&
            slot_CurrentIndex_ItemData.Usable == true &&
            _isHoldingCtrl)
        {
            var inventory = player.GetComponent<InventoryHolder>();

            slots[_currentIndex].AssignedInventorySlot.ItemData.UseItem();
            slots[_currentIndex].AssignedInventorySlot.SacarDeStack(slots[_currentIndex].AssignedInventorySlot.StackSize);
            if (slots[_currentIndex].AssignedInventorySlot.StackSize == 0)
            {
                slots[_currentIndex].AssignedInventorySlot.ClearSlot();
            }
            slots[_currentIndex].UpdateUISlot();
        }*/

        if (slot_CurrentIndex_ItemData != null &&
            slot_CurrentIndex_ItemData.Seed == true && 
            gridGhost.CheckCrop(gridGhost.finalPosition, 1) == true && 
            interactor._LookingAtDirt == true)
        {
            var inventory = player.GetComponent<InventoryHolder>();

            if (slot_CurrentIndex_ItemData.UseItem() == true)
            {
                slots[_currentIndex].AssignedInventorySlot.SacarDeStack(1);
                if (slots[_currentIndex].AssignedInventorySlot.StackSize == 0)
                {
                    slots[_currentIndex].AssignedInventorySlot.ClearSlot();
                }
            }
            slots[_currentIndex].UpdateUISlot();
        }

        if (slot_CurrentIndex_ItemData != null &&
            slot_CurrentIndex_ItemData.TreeSeed == true && 
            gridGhost.CheckCrop(gridGhost.finalPosition, 1) == true && 
            interactor._LookingAtDirt == false)
        {
            var inventory = player.GetComponent<InventoryHolder>();

            if (slots[_currentIndex].AssignedInventorySlot.ItemData.UseItem() == true)
            {
                slots[_currentIndex].AssignedInventorySlot.SacarDeStack(1);
                if (slots[_currentIndex].AssignedInventorySlot.StackSize == 0)
                {
                    slots[_currentIndex].AssignedInventorySlot.ClearSlot();
                }
            }
            slots[_currentIndex].UpdateUISlot();
        }
    }

    private void ChangeIndex(int direction)
    {
        slots[_currentIndex].ToggleHighlight();
        _currentIndex += direction;

        if (_currentIndex > _maxIndexSize) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = _maxIndexSize;

        slots[_currentIndex].ToggleHighlight();
    }

    private void SetIndex(int newIndex)
    {
        slots[_currentIndex].ToggleHighlight();
        if (newIndex < 0) _currentIndex = 0;
        if (newIndex > _maxIndexSize) newIndex = _maxIndexSize;

        _currentIndex = newIndex;
        slots[_currentIndex].ToggleHighlight();
        if (slots[_currentIndex].AssignedInventorySlot.ItemData != null)
        {
            NameDisplay.text = slots[_currentIndex].AssignedInventorySlot.ItemData.Nombre;
            if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo == false)
            {
                NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
                StartCoroutine(NameDisplay.GetComponent<NameDisplayController>().waiter());
                NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo = true;
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                NameDisplay.GetComponent<NameDisplayController>()._yaAnimo = false;
            }
            else if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo == true)
            {
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
            }
        }
    }
}