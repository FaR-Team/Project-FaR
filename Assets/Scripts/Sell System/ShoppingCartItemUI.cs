using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShoppingCartItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemText;


    public void SetItemText(string newString)
    {
        _itemText.text = newString;
    }
}
