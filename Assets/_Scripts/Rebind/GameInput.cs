using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameInput
{
    public const string PLAYER_PREFS_BINDINGS = "InputBindings";

    //public static GameInput Instance { get; private set; }

    public static event EventHandler OnBindingRebind;


    public enum Binding {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        Sprint,
        Pause,
        Inventory,
        Gamepad_Interact,
        Gamepad_Sprint,
        Gamepad_Pause,
        Gamepad_Inventory,
    }

    public static PlayerInput2 playerInputActions = new PlayerInput2();

        
/*        
    TODO ESTO DEBERÍA IR A OTRO SCRIPT QUE SEA MONO
playerInputActions = new PlayerInput2();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLABINDYER_PREFS_INGS));
        }

        playerInputActions.Player.Enable();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }
*/
   public static string GetBindingText(Binding binding)
   {
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Movement.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Movement.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Movement.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Movement.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interaction.bindings[0].ToDisplayString();
            case Binding.Sprint:
                return playerInputActions.Player.Sprint.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Inventory:
                return playerInputActions.Player.Inventory.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInputActions.Player.Interaction.bindings[1].ToDisplayString();
            case Binding.Gamepad_Sprint:
                return playerInputActions.Player.Sprint.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInputActions.Player.Pause.bindings[1].ToDisplayString();
            case Binding.Gamepad_Inventory:
                return playerInputActions.Player.Inventory.bindings[1].ToDisplayString();
        }
   }

   public static void RebindBinding(Binding binding, Action OnActionRebound)
   {
        playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Movement;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interaction;
                bindingIndex = 0;
                break;
            case Binding.Sprint:
                inputAction = playerInputActions.Player.Sprint;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Inventory:
                inputAction = playerInputActions.Player.Inventory;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                inputAction = playerInputActions.Player.Interaction;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Sprint:
                inputAction = playerInputActions.Player.Sprint;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Inventory:
                inputAction = playerInputActions.Player.Inventory;
                bindingIndex = 1;
                break;
        }        

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                callback.Dispose();
                playerInputActions.Player.Enable();
                OnActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
                var a = new GameInput();
                OnBindingRebind?.Invoke(a, EventArgs.Empty);
            })
            .Start(); //Igual, no estaría viendo dónde se usa el evento xd
   }
}
