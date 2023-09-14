using System.Collections.Generic;
using UnityEngine;

public class Saver : MonoBehaviour
{
    public static Dictionary<string, IPersistent> keyValuePairs;

    Saver instance;

    [SerializeField] private string nombreDeArchivo = "/Escena1.json";
    public static void Save()
    {
        var saver = new JsonParser("/Escena1.json", keyValuePairs);
    }

    private void Awake()
    {
        if(instance != null) Destroy(instance);
        
        instance = this;
    }

}
