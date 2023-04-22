using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using FaRUtils.Systems.DateTime;

public class UIController : MonoBehaviour
{
    [SerializeField] private ShopKeeperDisplay _shopKeeperDisplay;
    public GameObject player;
    public GameObject nameDisplayer;

    private void Awake()
    {
        _shopKeeperDisplay.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(_shopKeeperDisplay.gameObject.activeSelf)
        {
            nameDisplayer.SetActive(false);
        }
        else
        {
            nameDisplayer.SetActive(true);
        }
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopWindowRequested += DisplayShopWindow;    
    }

    void OnDisable()
    {
        ShopKeeper.OnShopWindowRequested -= DisplayShopWindow;    
    }

    private void DisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventory)
    {
        _shopKeeperDisplay.gameObject.SetActive(true);
        _shopKeeperDisplay.DisplayShopWindow(shopSystem, playerInventory);
    }
}
