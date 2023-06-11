using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class CropExplode : MonoBehaviour
{   
    public GameObject thisCropDirt;
    public GameObject ExplotionGameObject;

    public InventoryItemData ItemData;
    public GameObject player;

    //public Vector3 center;
    public float radius = 10;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        thisCropDirt = transform.parent.gameObject;
        //center = thisCropDirt.transform.position;
    }    
    
    public void Chau()
    {
        GetInventory().AñadirAInventario(ItemData, GetRandomInt());

        StartCoroutine(Destruir());
        /*Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        OnDrawGizmos();

        if (hitColliders  != null)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.tag == _objectName && hitCollider != this.gameObject.GetComponent<Collider>())
                {
                    hitCollider.gameObject.GetComponent<Carrot>().InteractOut();
                    TODO: No matarse (En swahilli).
                }
            }
        GameObject boom = Instantiate(Coso, pos, Quaternion.Euler(0,0,0));
        YaExploto = true;
        }*/
    }

    private int GetRandomInt()
    {
        return Random.Range(1, 5);
    }

    private PlayerInventoryHolder GetInventory()
    {
        return player.transform.GetComponent<PlayerInventoryHolder>();
    }

    IEnumerator Destruir()
    {
        //TODO: Get and do dirtAnimation.
        yield return new WaitForSeconds(2.5f);

        if (thisCropDirt.GetComponent<Animation>() != null)
        {
            thisCropDirt.GetComponent<Animation>().Play();
        }
        Instantiate(ExplotionGameObject, GetPosition(), Quaternion.Euler(0, 0, 0));
        
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject.GetComponent<Outline>());

        yield return new WaitForSeconds(0.5f);

        DirtSpawnerPooling.DeSpawn(DirtSpawnerPooling._DirtPrefab, thisCropDirt);
    }

    private Vector3 GetPosition()
    {
        return new Vector3(transform.root.GetChild(0).position.x, 2, transform.root.GetChild(0).position.z);
    }
}
