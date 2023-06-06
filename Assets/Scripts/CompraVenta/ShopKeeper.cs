using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;

[RequireComponent(typeof(UniqueID))]
public class ShopKeeper : MonoBehaviour, IInteractable
{
    public static ShopKeeper Instance { get; private set; }

    [SerializeField] private ShopItemList _shopItemsHeld;
    [SerializeField] private ShopSystem _shopSystem;
    public GameObject player;
    public GameObject ShopSystemUI;
    public GameObject InteractionPrompt { get; }

    public bool IsBuying;

    public static UnityAction<ShopSystem, PlayerInventoryHolder> OnShopWindowRequested;

    private void Awake() 
    {
        player = GameObject.FindWithTag("Player");
        _shopSystem = new ShopSystem(_shopItemsHeld.Items.Count, _shopItemsHeld.BuyMarkUp);

        foreach (var item in _shopItemsHeld.Items)
        {
            //Debug.Log($"{item.ItemData.Nombre}: {item.Amount}");
            _shopSystem.AddToShop(item.ItemData, item.Amount);
        }
    }

    private void Update() {
        if  (GameInput.playerInputActions.Player.Pause.WasPressedThisFrame() && IsBuying == true || GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() && IsBuying == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.GetComponent<FaRCharacterController>().enabled = true;
            player.GetComponent<DirtPlower>().enabled = true;
            ShopSystemUI.gameObject.SetActive(false);
            Time.timeScale = 1f;
            StartCoroutine(WaitJustSoTheInventoryDoesntOpenTwice());
        }
    }

    private IEnumerator WaitJustSoTheInventoryDoesntOpenTwice()
    {
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<PlayerInventoryHolder>().IsBuying = false;
        IsBuying = false;
    }


    public void Interact(Interactor interactor, out bool interactionSuccessful)
    {
        var playerInv = interactor.GetComponent<PlayerInventoryHolder>();

        if (playerInv != null)
        {
            OnShopWindowRequested?.Invoke(_shopSystem, playerInv);
            IsBuying = true;
            player.GetComponent<PlayerInventoryHolder>().IsBuying = true;
            player = GameObject.FindWithTag("Player");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.GetComponent<FaRCharacterController>().enabled = false;
            player.GetComponent<DirtPlower>().enabled = false;
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
