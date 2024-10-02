using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] public GameObject _uiPanel;
    public GameObject _BedPanel;
    public GameObject _ChestPanel;
    public GameObject _HousePanel;
    public GameObject _CropPanel;
    

    //[SerializeField] private TextMeshProUGUI _promptText;
    public bool IsDisplayed;

    private void Start()
    {
        //_uiPanel.SetActive(false);
    }


    public void SetUp()
    {
        _uiPanel.SetActive(true);
        IsDisplayed = true;
    }

    public void Close()
    {
        _uiPanel.SetActive(false);
        IsDisplayed = false;
    }
}
