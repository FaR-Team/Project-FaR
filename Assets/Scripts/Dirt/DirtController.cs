using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    public GameObject violeta;
    public GameObject Collider;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Violeta" || other.gameObject.transform.IsChildOf(transform)) return;

        Vector3 pos = other.gameObject.transform.position - violeta.transform.position;
        

        if (pos.x < 0)
        {
            dirtConnector().isOeste = true;
            dirtConnector().UpdateDirtPrefab();
        }
        if (pos.x > 0)
        {
            dirtConnector().isEste = true;
            dirtConnector().UpdateDirtPrefab();
        }
        if (pos.z > 0)
        {
            dirtConnector().isNorte = true;
            dirtConnector().UpdateDirtPrefab();
        }
        if (pos.z < 0)
        {
            dirtConnector().isSur = true;
            dirtConnector().UpdateDirtPrefab();
        }
    }

    DirtConnectorNew dirtConnector()
    {
        return Collider.GetComponent<DirtConnectorNew>();
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag != "Violeta" || other.gameObject.transform.IsChildOf(transform)) return;
        Vector3 pos = other.gameObject.transform.position - violeta.transform.position;

        if (pos.x < 0)
        {
            dirtConnector().isOeste = false;
            dirtConnector().UpdateDirtPrefab();
        }
        if (pos.x > 0)
        {
            dirtConnector().isEste = false;
            dirtConnector().UpdateDirtPrefab();
        }
        if (pos.z > 0)
        {
            dirtConnector().isNorte = false;
            dirtConnector().UpdateDirtPrefab();
        }
        if (pos.z < 0)
        {
            dirtConnector().isSur = false;
            dirtConnector().UpdateDirtPrefab();
        }
    }

}

