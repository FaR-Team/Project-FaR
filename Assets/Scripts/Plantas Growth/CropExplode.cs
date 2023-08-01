using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils;

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
    
    public void StartAnimationAndExplode()
    {
        //TODO: No matarse (En swahilli).
        if (this.GetComponent<Animation>() != null)
        {
            this.GetComponent<Animation>().Play();
        }
    }

    private int GetRandomInt()
    {
        return Random.Range(1, 5);
    }

    private PlayerInventoryHolder GetInventory()
    {
        return player.transform.GetComponent<PlayerInventoryHolder>();
    }

    public void Destruir()
    {
        if (thisCropDirt.GetComponentInChildren<Animation>() != null)
        {
            thisCropDirt.GetComponentInChildren<Animation>().Play();
        }
        Instantiate(ExplotionGameObject, GetPosition(), Quaternion.Euler(0, 0, 0));
        
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CarrotTuberInteraction>().InstantiateAndDropCarrots();
        Destroy(gameObject.GetComponent<Outline>());

        DirtSpawnerPooling.DeSpawn(DirtSpawnerPooling._DirtPrefab, thisCropDirt);
    }

    private Vector3 GetPosition()
    {
        return new Vector3(transform.parent.position.x, 2, transform.parent.position.z);
    }
}
