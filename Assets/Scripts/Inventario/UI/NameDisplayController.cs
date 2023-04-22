using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameDisplayController : MonoBehaviour
{
    public float timer = 5;
    public bool _yaAnimo = false;
    public bool _ContadorActivo = false;

    void Update()
    {

        if (timer > 0 ) timer -= Time.deltaTime;

        if (timer <= 0) _ContadorActivo = false;

        if (_ContadorActivo == false && _yaAnimo == false)
        {
            StartCoroutine(walter());
            _yaAnimo = true;
        }
    }

    public IEnumerator walter()
    {
        if (_ContadorActivo == true)
        {
            yield return new WaitForSeconds(1f);
            _ContadorActivo = false;
            _yaAnimo = false;
        }
        if (_ContadorActivo == false && _yaAnimo == false)
        {
            this.GetComponent<Animation>().Play("NameDisplaySalir");
            _yaAnimo = true;
        }
    }
}
