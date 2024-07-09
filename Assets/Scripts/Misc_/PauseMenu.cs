using FaRUtils.FPSController;
using IngameDebugConsole;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public static PauseMenu Instance;

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject bindingMenuUI;
    public GameObject player;

    public CambiarEscena _cambiarEscena;
    public SleepHandler sleepHandler;
    public GameObject UI;
    public GameObject Options;
    public GameObject PhysicsGun;
    public Button resumeButton;

    public AudioSource Music;

    public FPSLimit FPSLimit;
    public TextMeshProUGUI FPSText;

    public DebugLogManager debugLogManager;
    //public GameObject Hotbar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        sleepHandler = SleepHandler.Instance;

        if (FPSLimit.target == 0)
        {
            FPSLimit.target = 60;
            PlayerPrefs.SetInt("TargetaFPS", 60);
        }
        FPSLimit.target = PlayerPrefs.GetInt("TargetaFPS");
        FPSText.text = "FPS: " + FPSLimit.target;
    }

    private void Update()
    {
        if (GameInput.playerInputActions.Player.Pause.WasPressedThisFrame() &&
            !PlayerInventoryHolder.isInventoryOpen &&
            !ShopIsBuying() &&
            !sleepHandler._isSleeping &&
            !DebugLogManager.Instance.isOnConsole)
        {
            if (GameIsPaused)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    private bool ShopIsBuying()
    {
        if (ShopKeeper.Instance == null)
        {
            return false;
        }
        else
        {
            return ShopKeeper.Instance.IsBuying;
        }
    }

    public void ClosePauseMenu()
    {
        UI.SetActive(true);
        //Hotbar.SetActive(true);
        if (Music.clip != null)
        {
            Music.UnPause();
        }
        GameInput.playerInputActions.Player.Inventory.Enable();
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);

        GameIsPaused = false;
        Unpause();
    }

    public void Unpause()
    {
        PhysicsGun.SetActive(true);
        player.GetComponent<FaRCharacterController>().enabled = true;
        player.GetComponent<Interactor>().enabled = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenPauseMenu()
    {
        UI.SetActive(false);
        //Hotbar.SetActive(true);
        if (Music.clip != null)
        {
            Music.Pause();
        }
        GameIsPaused = true;
        GameInput.playerInputActions.Player.Inventory.Disable();
        pauseMenuUI.SetActive(true);

        resumeButton.Select();
        Pause();
    }

    public void Pause()
    {
        PhysicsGun.SetActive(false);
        player.GetComponent<FaRCharacterController>().enabled = false;
        player.GetComponent<Interactor>().enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AshudaMabel()
    {
        // Cambiar para que Tpee al SpawnPoint
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = Vector3.zero;
        player.GetComponent<CharacterController>().enabled = true;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("Menu");
        Debug.Log("Aún no hay menú xd");
    }

    public void OptionsMenu()
    {
        pauseMenuUI.SetActive(false);
        this.GetComponent<OptionsMenu>().isOptionsMenuOpen = true;
        Options.SetActive(true);
    }

    public void OptionsMenuBack()
    {
        pauseMenuUI.SetActive(true);
        this.GetComponent<OptionsMenu>().isOptionsMenuOpen = false;
        Options.SetActive(false);
    }

    public void BindingsMenu()
    {
        pauseMenuUI.SetActive(false);
        Options.SetActive(false);
        bindingMenuUI.SetActive(true);
    }

    public void BindingsMenuBack()
    {
        pauseMenuUI.SetActive(false);
        bindingMenuUI.SetActive(false);
        Options.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
