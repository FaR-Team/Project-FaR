using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadBinding : MonoBehaviour
{
    // Start is called before the first frame update
    void  Awake()
    {
        if (PlayerPrefs.HasKey(GameInput.PLAYER_PREFS_BINDINGS))
        {
            GameInput.playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(GameInput.PLAYER_PREFS_BINDINGS));
        }

        GameInput.playerInputActions.Player.Enable();

        if (PlayerPrefs.HasKey(GameInput.PLAYER_PREFS_BINDINGS))
        {
            GameInput.playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(GameInput.PLAYER_PREFS_BINDINGS));
        }
    }
}