using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class spawnPoint : MonoBehaviour
{
    public GameObject go;

    void Start()
    {
        go = GameObject.Find("FPSController");
        go.GetComponent<FirstPersonController>().enabled = false;
        go.GetComponent<CharacterController>().enabled = false;
        go.transform.position = gameObject.transform.position;
        go.GetComponent<FirstPersonController>().enabled = true;
        go.GetComponent<CharacterController>().enabled = true;
    }
}
