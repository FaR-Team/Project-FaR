using FaRUtils.FPSController;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueID))]
public class ShopKeeper : MonoBehaviour, IInteractable
{
    public static ShopKeeper Instance { get; private set; }

    [SerializeField] private ShopItemList _shopItemsHeld;
    [SerializeField] private ShopSystem _shopSystem;

    private FaRCharacterController player;

    public GameObject ShopSystemUI;
    public GameObject InteractionPrompt { get; }

    public bool IsBuying;

    public static UnityAction<ShopSystem, PlayerInventoryHolder> OnShopWindowRequested;

    private void Awake()
    {
        SetInstance();

        player = FaRCharacterController.instance;
        _shopSystem = new ShopSystem(_shopItemsHeld.Items.Count, _shopItemsHeld.BuyMarkUp);

        foreach (var item in _shopItemsHeld.Items)
        {
            //Debug.Log($"{item.ItemData.Nombre}: {item.Amount}");
            _shopSystem.AddToShop(item.ItemData, item.Amount);
        }
    }

    private void SetInstance()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
        }
    }

    private void Update()
    {
        bool pauseOrInventoryWasPressed = (GameInput.playerInputActions.Player.Pause.WasPressedThisFrame() ||
                    GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame());

        if (IsBuying && pauseOrInventoryWasPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            player.enabled = true;

            ShopKeeperDisplay.Instance.gameObject.SetActive(false);

            Time.timeScale = 1f;
            StartCoroutine(WaitJustSoTheInventoryDoesntOpenTwice());
        }
    }

    private IEnumerator WaitJustSoTheInventoryDoesntOpenTwice()
    {
        yield return new WaitForSeconds(0.1f);

        PlayerInventoryHolder.IsBuying = false;
        IsBuying = false;
    }


    public void Interact(Interactor interactor, out bool interactionSuccessful)
    {
        if (interactor.TryGetComponent<PlayerInventoryHolder>(out var playerInv))
        {
            OnShopWindowRequested?.Invoke(_shopSystem, playerInv);
            IsBuying = true;
            PlayerInventoryHolder.IsBuying = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.enabled = false;
            Time.timeScale = 1f;
        }
        else
        {
            interactionSuccessful = false;
            Debug.LogError("No existe inventario del Jugador");
        }

        interactionSuccessful = true;
    }

    public void InteractOut()
    {
        Debug.Log(null);
    }

    public void EndInteraction()
    {

    }
}
