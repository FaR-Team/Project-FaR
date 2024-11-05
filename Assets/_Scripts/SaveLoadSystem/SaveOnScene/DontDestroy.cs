using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[RequireComponent(typeof(UniqueID))]
public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        var objects = Object.FindObjectsOfType<DontDestroy>(true);
        for (int i = 0; i < objects.Length; i++)
        {
            var GODontDestroy = objects[i];

            if (GODontDestroy != this)
            {
                if(GODontDestroy.GetComponent<UniqueID>().ID == this.gameObject.GetComponent<UniqueID>().ID)
                {
                    var components = gameObject.GetComponents<Component>();
                    foreach (var component in components)
                    {
                        if (component is MonoBehaviour behaviour)
                        {
                            behaviour.enabled = false;
                        }
                    }
                    
                    Destroy(gameObject);
                    this.LogSuccess("Destroyed "  + GODontDestroy.gameObject.name);
                }
            }
        }
        DontDestroyOnLoad(gameObject);
    }
}
