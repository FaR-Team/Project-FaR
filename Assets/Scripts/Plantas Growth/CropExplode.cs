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
        thisCropDirt = transform.parent.parent.gameObject;
        //center = thisCropDirt.transform.position;
    }    
    
    public void StartAnimationAndExplode()
    {
        //TODO: No matarse (En swahilli).
        if (this.GetComponent<Animation>() != null)
        {
            this.GetComponent<Animation>().Play();
        }
        if (thisCropDirt.GetComponentInChildren<Animation>() != null)
        {
            thisCropDirt.GetComponentInChildren<Animation>().Play();
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

    public IEnumerator Destruir()
    {
        thisCropDirt.transform.position = new Vector3(thisCropDirt.transform.position.x, -2, thisCropDirt.transform.position.z);
        Instantiate(ExplotionGameObject, GetPosition(), Quaternion.identity);
        
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        GetComponent<CarrotTuberInteraction>().InstantiateAndDropCarrots();
        Destroy(gameObject.GetComponent<Outline>());

        yield return new WaitForSeconds(0.2f);
        DirtSpawnerPooling.DeSpawn(DirtSpawnerPooling._DirtPrefab, thisCropDirt);
        Destroy(gameObject.transform.parent.gameObject);
    }

    private Vector3 GetPosition()
    {
        return new Vector3(transform.parent.parent.position.x, 2, transform.parent.parent.position.z);
    }
}
