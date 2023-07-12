using UnityEngine;

public class FallingFruit : MonoBehaviour
{
    // Esto seria tirar las frutas al piso y permitir al jugador recogerlas.
    public void FallFruit()
    {
        GetComponent<ItemPickUp>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }
    //TO DO: Poner una fuerza para lanzarlo levemente hacia los costados.
}