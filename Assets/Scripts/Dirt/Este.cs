using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Este : MonoBehaviour
{
    public GameObject Colliders;

    void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Violeta")
            {
                Colliders.GetComponent<DirtConnector>().isEste = true;
                Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            Colliders.GetComponent<DirtConnector>().isEste = false;
            Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
        }
    }

    private void OnDisable() 
    {
        Colliders.GetComponent<DirtConnector>().isEste = false;
        Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
    }

    private void OnDestroy() 
    {
        Colliders.GetComponent<DirtConnector>().isEste = false;
        Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
    }
}
