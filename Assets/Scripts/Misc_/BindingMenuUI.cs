using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BindingMenuUI : MonoBehaviour
{
    public static BindingMenuUI Instance { get; private set; }


    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private Button interactionButton;
    [SerializeField] private TextMeshProUGUI sprintText;
    [SerializeField] private Button sprintButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private TextMeshProUGUI gamepadInteractionText;
    [SerializeField] private Button gamepadInteractionButton;
    [SerializeField] private TextMeshProUGUI gamepadSprintText;
    [SerializeField] private Button gamepadSprintButton;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI gamepadInventoryText;
    [SerializeField] private Button gamepadInventoryButton;
    [SerializeField] private Transform pressToRebindKeyTransform;


    private Action onCloseButtonAction;


    private void Awake() 
    {
        Instance = this;

        moveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Up); });
        moveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Down); });
        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Left); });
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Right); });
        interactionButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        sprintButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Sprint); });
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
        inventoryButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Inventory); });
        gamepadInteractionButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Interact); });
        gamepadSprintButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Sprint); });
        gamepadPauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Pause); });
        gamepadInventoryButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Inventory); });
    }

    private void Start() 
    {
        UpdateVisual();

        HidePressToRebindKey();
    }


    private void UpdateVisual()
    {
        moveUpText.text = GameInput.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.GetBindingText(GameInput.Binding.Move_Right);
        interactionText.text = GameInput.GetBindingText(GameInput.Binding.Interact);
        sprintText.text = GameInput.GetBindingText(GameInput.Binding.Sprint);
        pauseText.text = GameInput.GetBindingText(GameInput.Binding.Pause);
        inventoryText.text = GameInput.GetBindingText(GameInput.Binding.Inventory);
        gamepadInteractionText.text = GameInput.GetBindingText(GameInput.Binding.Gamepad_Interact);
        gamepadSprintText.text = GameInput.GetBindingText(GameInput.Binding.Gamepad_Sprint);
        gamepadPauseText.text = GameInput.GetBindingText(GameInput.Binding.Gamepad_Pause);
        gamepadInventoryText.text = GameInput.GetBindingText(GameInput.Binding.Gamepad_Inventory);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
