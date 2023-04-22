using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntrarCasa : MonoBehaviour
{
    public int buildIndex;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag =="Player")
        {

            SceneManager.LoadScene(buildIndex);
        }
    }
}

