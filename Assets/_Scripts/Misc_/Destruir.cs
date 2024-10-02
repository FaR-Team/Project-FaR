using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruir : MonoBehaviour
{

    void Update()
    {
        StartCoroutine(DestruirCosa());        
    }

    IEnumerator DestruirCosa()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }
}
