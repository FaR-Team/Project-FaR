using UnityEngine;
using System.Collections;
using FaRUtils;

public class FallingFruit : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    // Esto seria tirar las frutas al piso y permitir al jugador recogerlas.
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
        //TODO: Antes tiene que instanciar el número aleatorio de tubérculos que te vaya a dar, 
        //habría que verlo cuál es el número en el SO, y con ese mismo sacar el objeto a instanciar(?

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);

        GetComponent<Rigidbody>().isKinematic = false;
        Vector3 force = transform.forward;
        force = new Vector3(force.x, 1, force.z);
        GetComponent<Rigidbody>().AddForce(0, 0, speed, ForceMode.Impulse);

        yield return new WaitForSeconds(0.1f);

        GetComponent<ItemPickUp>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<Outline>().enabled = false;
    }

    IEnumerator DropFruit()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<ItemPickUp>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Outline>().enabled = false;
    }
}