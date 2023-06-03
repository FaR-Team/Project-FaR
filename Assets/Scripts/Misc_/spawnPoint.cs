using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.FPSController;

public class spawnPoint : MonoBehaviour
{
    public GameObject go;

    void Start()
    {
        go = GameObject.Find("FPSController");
        go.GetComponent<FaRCharacterController>().enabled = false;
        go.GetComponent<CharacterController>().enabled = false;
        go.transform.position = gameObject.transform.position;
        go.GetComponent<FaRCharacterController>().enabled = true;
        go.GetComponent<CharacterController>().enabled = true;
    }
}
