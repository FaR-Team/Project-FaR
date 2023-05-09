using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    public GameObject violeta;
    public GameObject Collider;

    Dictionary<string, string> gameObjectsDict = new Dictionary<string, string>();

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Violeta" || other.gameObject.transform.IsChildOf(transform)) return;

        Vector3 pos = other.gameObject.transform.position - violeta.transform.position;
        
        string iD = other.GetComponent<UniqueID>().ID;
        

        if (pos.x < 0)
        {
            dirtConnector().isOeste = true;
            dirtConnector().UpdateDirtPrefab();
            gameObjectsDict.Add("Oeste", iD);
        }
        if (pos.x > 0)
        {
            dirtConnector().isEste = true;
            dirtConnector().UpdateDirtPrefab();
            gameObjectsDict.Add("Este", iD);
        }
        if (pos.z > 0)
        {
            dirtConnector().isNorte = true;
            dirtConnector().UpdateDirtPrefab();
            gameObjectsDict.Add("Norte", iD);
        }
        if (pos.z < 0)
        {
            dirtConnector().isSur = true;
            dirtConnector().UpdateDirtPrefab();
            gameObjectsDict.Add("Sur", iD);
        }
    }

    DirtConnectorNew dirtConnector()
    {
        return Collider.GetComponent<DirtConnectorNew>();
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag != "Violeta" || other.gameObject.transform.IsChildOf(transform)) return;
        
        dirtConnector().isSur = ObjetoConValor("Sur", other.GetComponent<UniqueID>().ID);
        dirtConnector().isOeste = ObjetoConValor("Oeste", other.GetComponent<UniqueID>().ID);
        dirtConnector().isEste = ObjetoConValor("Este", other.GetComponent<UniqueID>().ID);
        dirtConnector().isNorte = ObjetoConValor("Norte", other.GetComponent<UniqueID>().ID);

        dirtConnector().UpdateDirtPrefab();

    }
    bool ObjetoConValor(string clave, string valor)
    {
        foreach (KeyValuePair<string, string> kvp in gameObjectsDict)
        {
            if (kvp.Key == clave && kvp.Value == valor) return true;
        }
        return false;
    }
}

