using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sur : MonoBehaviour
{
    public GameObject Colliders;

    void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Violeta")
            {
                Colliders.GetComponent<DirtConnector>().isSur = true;
                Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            Colliders.GetComponent<DirtConnector>().isSur = false;
            Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
        }
    }

    private void OnDisable() 
    {
        Colliders.GetComponent<DirtConnector>().isSur = false;
        Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
    }

    private void OnDestroy() 
    {
        Colliders.GetComponent<DirtConnector>().isSur = false;
        Colliders.GetComponent<DirtConnector>().UpdateDirtPrefab();
    }
}
