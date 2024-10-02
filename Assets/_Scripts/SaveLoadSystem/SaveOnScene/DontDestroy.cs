using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class DontDestroy : MonoBehaviour
{
    /*[HideInInspector]
    public string objID;
    void Awake() { objID = name + transform.position.ToString(); }*/
    
    void Awake()
    {
        var objects = Object.FindObjectsOfType<DontDestroy>(true);
        for (int i = 0; i < objects.Length; i++)
        {
            var GODontDestroy = objects[i];

            if (GODontDestroy != this)
            {
                /*if (GODontDestroy.objID == objID)
                {
                    Destroy(gameObject);
                }*/
                if(GODontDestroy.GetComponent<UniqueID>().ID == this.gameObject.GetComponent<UniqueID>().ID)
                {
                    Destroy(gameObject);
                }
            }

        }
        DontDestroyOnLoad(gameObject);
    }
}
