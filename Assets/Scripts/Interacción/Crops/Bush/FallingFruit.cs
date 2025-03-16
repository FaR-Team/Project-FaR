using UnityEngine;
using System.Collections;
using FaRUtils;

public class FallingFruit : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    private MaterialPropertyBlock _propertyBlock;
    // Esto seria tirar las frutas al piso y permitir al jugador recogerlas.

    void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void FallFruit()
    {
        StartCoroutine(DropFruit());
    }

    public void FallTuber()
    {
        StartCoroutine(LaunchTuber());
    }

    IEnumerator LaunchTuber()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);

        GetComponent<Rigidbody>().isKinematic = false;
        Vector3 force = transform.forward;
        force = new Vector3(force.x, 1, force.z);
        GetComponent<Rigidbody>().AddForce(force * speed, ForceMode.Impulse);

        yield return new WaitForSeconds(0.1f);

        GetComponent<ItemPickUp>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            _propertyBlock.SetFloat("_UseOutline", 0);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    IEnumerator DropFruit()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            _propertyBlock.SetFloat("_UseOutline", 1);
            renderer.SetPropertyBlock(_propertyBlock);
        }
        
        yield return new WaitForSeconds(0.5f);
        GetComponent<ItemPickUp>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        
        if (renderer != null)
        {
            _propertyBlock.SetFloat("_UseOutline", 0);
            renderer.SetPropertyBlock(_propertyBlock);
        }
        
        gameObject.layer = 0;
        this.transform.parent = null;
    }
}