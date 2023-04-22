using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oeste : MonoBehaviour
{
    public GameObject Colliders;

    void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Violeta")
            {
                Colliders.GetComponent<DirtConnector>().isOeste = true;
                Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            Colliders.GetComponent<DirtConnector>().isOeste = false;
            Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
        }
    }

    private void OnDisable() 
    {
        Colliders.GetComponent<DirtConnector>().isOeste = false;
        Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
    }

    private void OnDestroy() 
    {
        Colliders.GetComponent<DirtConnector>().isOeste = false;
        Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
    }
}