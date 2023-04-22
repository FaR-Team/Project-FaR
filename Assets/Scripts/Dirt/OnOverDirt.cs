using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOverDirt : MonoBehaviour
{
    public bool isDirt = false;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Dirt")
        {
            isDirt = true;
        }
        else
        {
            isDirt = false;
        }
    }
}