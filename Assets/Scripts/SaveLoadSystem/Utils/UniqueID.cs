using System;
using Unity.Collections;
using UnityEngine;


[System.Serializable]
[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    [ReadOnly, SerializeField] private string _ID;
    [SerializeField] private static SerializableDictionary<string, GameObject> IDdataBase = new SerializableDictionary<string, GameObject>();
    [SerializeField] private bool permanentID = false;

    public string ID => _ID;
    private void Awake()
    {
        
        if (IDdataBase == null) IDdataBase = new SerializableDictionary<string, GameObject>();
       
        if (IDdataBase.ContainsKey(_ID))
        {
            if (permanentID) return;
            Generate();
        }
        else
        {
            IDdataBase.Add(_ID, this.gameObject);
        }
       
    }

    private void OnDestroy()
    {
        if (IDdataBase.ContainsKey(_ID))
        {
            IDdataBase.Remove(_ID);
        }
    }

    void Reset()
    {
        if (!permanentID)
        {
            Generate();
        }
    }

[ContextMenu("Generar ID")]
    public void Generate()
    {
        _ID = Guid.NewGuid().ToString();
        IDdataBase.Add(_ID, this.gameObject);
    }
    
}
