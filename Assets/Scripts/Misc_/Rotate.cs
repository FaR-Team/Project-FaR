using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public int vel;
    void Update()
    {
        this.gameObject.transform.Rotate(0f, vel, 0f);       
    }
}
