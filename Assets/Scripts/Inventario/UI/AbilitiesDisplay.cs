using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilitiesDisplay : StaticInventoryDisplay
{
    private int _maxIndexSize = 9;
    private int _currentIndex = 0;

    private PlayerInput2 _playerControls;

    private void Awake() {
        _playerControls = new PlayerInput2();
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

    private void Update()
    {
        if (_playerControls.Player.MouseWheel.ReadValue<float>() > 0.1f) ChangeIndex(-1);
        if (_playerControls.Player.MouseWheel.ReadValue<float>() < -0.1f) ChangeIndex(1);
    }

    private void UseItem(InputAction.CallbackContext obj)
    {
        if (slots[_currentIndex].AssignedInventorySlot.ItemData != null && slots[_currentIndex].AssignedInventorySlot.ItemData.Usable == true) slots[_currentIndex].AssignedInventorySlot.ItemData.UseItem();
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
    }
}