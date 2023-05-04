using System.Collections;
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

        Debug.Log(("La pos en x: " + pos.x) + ("     La pos en z: " + pos.z));
        if (pos.x < 0)
        {
            Collider.GetComponent<DirtConnector>().isOeste = true;
            Collider.GetComponent<DirtConnector>().UpdateDirtPrefab();
            Debug.Log("Oeste");
        }
        if (pos.x > 0)
        {
            Collider.GetComponent<DirtConnector>().isEste = true;
            Collider.GetComponent<DirtConnector>().UpdateDirtPrefab();
            Debug.Log("este");
        }
        if (pos.z > 0)
        {
            Collider.GetComponent<DirtConnector>().isNorte = true;
            Collider.GetComponent<DirtConnector>().UpdateDirtPrefab();
            Debug.Log("norte");
        }
        if (pos.z < 0)
        {
            Collider.GetComponent<DirtConnector>().isSur = true;
            Collider.GetComponent<DirtConnector>().UpdateDirtPrefab();
            Debug.Log("sur");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag != "Violeta" || other.gameObject.transform.IsChildOf(transform)) return;
        
        //Collider.GetComponent<DirtConnector>().isEste = false;
        //Collider.GetComponent<DirtConnector>().UpdateDirtPrefab();
        
    }
}

