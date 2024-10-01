using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DirtConnector : MonoBehaviour
{

    [Header("Tierras")]


    [SerializeField]
    public GameObject[] listOfDirts = new GameObject[16];

    [Header("Booleanos")]
    public bool isSur = false;
    public bool isOeste = false;
    public bool isNorte = false;
    public bool isEste = false;

    public void SetSelectedBool(GameObject[] goList, int selectedIndex)
    {
        // elemento en "true"
        
        
        goList[selectedIndex].SetActive(true);

        // demás elementos en "false"
        for (int i = 0; i < goList.Length; i++)
        {
            if (i != selectedIndex)
            {
                goList[i].SetActive(false);
            }
        }
    }

    public void UpdateDirtPrefab()
    {
        int value = Convert.ToInt32(isSur) << 3 | Convert.ToInt32(isOeste) << 2 | Convert.ToInt32(isNorte) << 1 | Convert.ToInt32(isEste);
        SetSelectedBool(listOfDirts, value);
    }
}
