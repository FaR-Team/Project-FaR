using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtAnimation : MonoBehaviour
{
    private GameObject parent;

    void Awake()
    {
        parent = transform.parent.gameObject;        
    }
    
    public void PrepareToGetDown()
    {
        parent.GetComponent<Dirt>().GetDown();
    }
}
