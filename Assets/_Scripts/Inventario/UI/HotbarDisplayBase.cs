using TMPro;
using UnityEngine;

public class HotbarDisplayBase : StaticInventoryDisplay
{
    public static int _maxIndexSize = 9;
    public static int _currentIndex = 0;

    public static bool _isHolding = false;
    public static bool _isHoldingCtrl = false;

    public PlayerInput2 _playerControls;

    public TextMeshProUGUI NameDisplay;
    public GameObject player;

    protected NameDisplayController nameDisplayController;

    protected override void Start()
    {
        base.Start();
        nameDisplayController = NameDisplay.GetComponent<NameDisplayController>();
    }

    public virtual void Update()
    {
        if ((MouseWheelValue() > 0.1f || MouseWheelValue() < -0.1f) && IsNotGrabingNorPausedNorConsole())
        {
            if (MouseWheelValue() > 0.1f && !CurrentIndexIsSpecialSlotAndYouAreHoldingCtrl()) ChangeIndex(-1);
            if (MouseWheelValue() < -0.1f && !CurrentIndexIsSpecialSlotAndYouAreHoldingCtrl()) ChangeIndex(1);

            DoChangeNameDisplay();
        }
    }

    public static bool CurrentIndexIsSpecialSlotAndYouAreHoldingCtrl()
    {
        return (_currentIndex == 10 && _isHoldingCtrl);
    }

    protected InventorySlot_UIBasic AbilitySlot()
    {
        return slots[10];
    }
    protected InventorySlot GetAssignedInventorySlot()
    {
        return SlotCurrentIndex().AssignedInventorySlot;
    }

    protected InventoryItemData GetItemData()
    {
        if (GetAssignedInventorySlot().ItemData != null)
        {
            return GetAssignedInventorySlot().ItemData;
        }
        else
        {
            return null;
        }
    }

    protected InventorySlot_UIBasic SlotCurrentIndex()
    {
        return slots[_currentIndex];
    }

    protected void ChangeIndex(int direction)
    {
        SlotCurrentIndex().ToggleHighlight();
        _currentIndex += direction;

        if (_currentIndex > _maxIndexSize) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = _maxIndexSize;

        SlotCurrentIndex().ToggleHighlight();
    }

    protected PlayerInput2.PlayerActions GetPlayerControls()
    {
        return _playerControls.Player;
    }

    protected void DoChangeNameDisplay()
    {
        if (GetItemData() == null) return;

        NameDisplay.text = GetItemData().Nombre;

        if (nameDisplayController._ContadorActivo)
        {
            nameDisplayController.timer = 2;
        }
        else
        {
            NameDisplay.GetComponent<Animation>().Play("NameDisplayEntrar");
            nameDisplayController._ContadorActivo = true;
            nameDisplayController.timer = 2;
            nameDisplayController._yaAnimo = false;
            StartCoroutine(nameDisplayController.waiter());
        }
    }

    protected bool IsNotGrabingNorPausedNorConsole()
    {
        return (!PhysicsGunInteractionBehavior.isGrabbingObject &&
                    !PauseMenu.GameIsPaused &&
                    !PlayerInventoryHolder.IsBuying &&
                    !IngameDebugConsole.DebugLogManager.Instance.isOnConsole);
    }
    
    protected float MouseWheelValue()
    {
        return GetPlayerControls().MouseWheel.ReadValue<float>();
    }

}