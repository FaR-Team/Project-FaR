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

    Dirt dirtToTest;

    private void Awake()
    {
        _playerControls = GameInput.playerInputActions;
       
        if(telequinesis == null ) telequinesis = GameObject.FindWithTag("Telequinesis");
        if(player == null) player = GameObject.FindWithTag("Player");

        PlayerInv = player.GetComponent<PlayerInventoryHolder>();

        physicsGun = telequinesis.GetComponent<PhysicsGunInteractionBehavior>();
    }

    protected override void Start()
    {
        base.Start();

        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;

        SlotCurrentIndex().ToggleHighlight();
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

    private PlayerInput2.PlayerActions GetPlayerControls()
    {
        return _playerControls.Player;
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
        if (_isHoldingCtrl && _isHolding && interactor.IsLookingAtStore) //TODO: && GetPlayerControls().UseItem.WasPressedThisFrame()
        {
            if (GetItemData() == null || GetItemData().Sellable == false) return;

            if (GetItemData().Seed == false && GetItemData().Usable == true)
            {
                int i = 0;
                while (i < GetAssignedInventorySlot().StackSize)
                {
                    GetItemData().UseItem();
                    GetAssignedInventorySlot().SacarDeStack(1);
                }
                if (GetAssignedInventorySlot().StackSize == 0)
                {
                    GetAssignedInventorySlot().ClearSlot();
                }
                SlotCurrentIndex().UpdateUISlot();
            }
        }
    }

    private InventoryItemData GetItemData()
    {
        return GetAssignedInventorySlot().ItemData;
    }

    private InventorySlot GetAssignedInventorySlot()
    {
        return SlotCurrentIndex().AssignedInventorySlot;
    }
    private InventorySlot_UI SlotCurrentIndex()
    {
        return slots[_currentIndex];
    }

    private void Update()
    {

        float MouseWheelValue()
        {
            return GetPlayerControls().MouseWheel.ReadValue<float>();
        }

        if((MouseWheelValue() > 0.1f || MouseWheelValue() < -0.1f) && 
            physicsGun.isGrabbingObject == false &&
            PauseMenu.GameIsPaused == false &&
            PlayerInv.IsBuying == false &&
            IngameDebugConsole.DebugLogManager.Instance.isOnConsole == false)
        {
            if (MouseWheelValue() > 0.1f) ChangeIndex(-1);
            if (MouseWheelValue() < -0.1f) ChangeIndex(1);

            if (GetItemData() == null) return;

            NameDisplay.text = GetItemData().Nombre;
            if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo)
            {
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
            }
            else
            {
                NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
                NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo = true;
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                NameDisplay.GetComponent<NameDisplayController>()._yaAnimo = false;
                StartCoroutine(NameDisplay.GetComponent<NameDisplayController>().waiter());
            }
        }

        if(GetPlayerControls().HotbarRight.WasPressedThisFrame() && 
            physicsGun.isGrabbingObject == false &&
            PauseMenu.GameIsPaused == false &&
            PlayerInv.IsBuying == false &&
            IngameDebugConsole.DebugLogManager.Instance.isOnConsole == false)
        {
            ChangeIndex(1);

            if (GetItemData() == null) return;

            NameDisplay.text = GetItemData().Nombre;
            if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo)
            {
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
            }
            else
            {
                NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
                NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo = true;
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                NameDisplay.GetComponent<NameDisplayController>()._yaAnimo = false;
                StartCoroutine(NameDisplay.GetComponent<NameDisplayController>().waiter());
            }
        }

        if(GetPlayerControls().HotbarLeft.WasPressedThisFrame() && 
            physicsGun.isGrabbingObject == false &&
            PauseMenu.GameIsPaused == false &&
            PlayerInv.IsBuying == false &&
            IngameDebugConsole.DebugLogManager.Instance.isOnConsole == false)
        {
            ChangeIndex(-1);

            if (GetItemData() == null) return;

            NameDisplay.text = GetItemData().Nombre;
            if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo)
            {
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
            }
            else
            {
                NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
                NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo = true;
                NameDisplay.GetComponent<NameDisplayController>().timer = 2;
                NameDisplay.GetComponent<NameDisplayController>()._yaAnimo = false;
                StartCoroutine(NameDisplay.GetComponent<NameDisplayController>().waiter());
            }
        }


        if (_isHoldingCtrl)
        {
            SellAll();
        }
        if (GetItemData() == null)
        {
            hoe.SetActive(false);
            bucket.SetActive(false);
            hand.SetActive(true);
            return;
        }


        if (GetItemData().IsHoe == true)
        {
            hoe.SetActive(true);
            bucket.SetActive(false);
            hand.SetActive(false);
        }
        else if (
            (GetItemData().Sellable == false &&
            GetItemData().Seed == false &&
            GetItemData().Usable == false &&
            GetItemData().Tool == false)

            ||

            GetItemData().Usable == true)
        {
            hoe.SetActive(false);
            bucket.SetActive(false);
            hand.SetActive(true);
        }

        if (GetItemData().IsBucket == true)
        {
            hoe.SetActive(false);
            bucket.SetActive(true);
            hand.SetActive(false);
        }
    }
    

    private void Holdear()
    {
        if (_isHolding && !_isHoldingCtrl && interactor.IsLookingAtStore)
        {
            if (GetItemData() == null) return;

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
        }
    }

    private Vector3 previousFinalPosition;

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


    private void UseItem(InputAction.CallbackContext obj)
    {
        if (GetItemData() == null) return;
        
        
        if (GetItemData().Usable == true &&
            GetItemData().Seed == false &&
            GetItemData().Sellable == false &&
            GetItemData().TreeSeed == false)
            //La música de relee es una poronga

        { GetItemData().UseItem(); }

        /*if (GetItemData() != null &&
            GetItemData().Sellable == true &&
            GetItemData().Seed == false &&
            GetItemData().Usable == true &&
            _isHoldingCtrl)
        {
            var inventory = player.GetComponent<InventoryHolder>();

            GetItemData().UseItem();
            GetAssignedInventorySlot().SacarDeStack(GetAssignedInventorySlot().StackSize);
            if (GetAssignedInventorySlot().StackSize == 0)
            {
                GetAssignedInventorySlot().ClearSlot();
            }
            slotCurrentIndex().UpdateUISlot();
        }*/

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

    private void ChangeIndex(int direction)
    {
        SlotCurrentIndex().ToggleHighlight();
        _currentIndex += direction;

        if (_currentIndex > _maxIndexSize) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = _maxIndexSize;

        SlotCurrentIndex().ToggleHighlight();
    }

    private void SetIndex(int newIndex)
    {
        SlotCurrentIndex().ToggleHighlight();
        if (newIndex < 0) _currentIndex = 0;
        if (newIndex > _maxIndexSize) newIndex = _maxIndexSize;

        _currentIndex = newIndex;
        SlotCurrentIndex().ToggleHighlight();

        if (GetItemData() == null) return;

        NameDisplay.text = GetItemData().Nombre;

        if (NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo)
        {
            NameDisplay.GetComponent<NameDisplayController>().timer = 2;
        }
        else
        {
            NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
            StartCoroutine(NameDisplay.GetComponent<NameDisplayController>().waiter());
            NameDisplay.GetComponent<NameDisplayController>()._ContadorActivo = true;
            NameDisplay.GetComponent<NameDisplayController>().timer = 2;
            NameDisplay.GetComponent<NameDisplayController>()._yaAnimo = false;
        }
    }
}
