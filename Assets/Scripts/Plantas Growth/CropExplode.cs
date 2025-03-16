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
        if (TryGetComponent(out Animator anim))
        {
            anim.SetTrigger("Harvest");
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
        
        var skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        var standardMesh = GetComponentInChildren<MeshRenderer>();
        
        if (skinnedMesh != null)
        {
            skinnedMesh.enabled = false;
            GetComponent<SkinnedMeshTuberInteraction>()?.InstantiateAndDropFruits();
        }
        else if (standardMesh != null) 
        {
            standardMesh.enabled = false;
            GetComponent<MeshTuberInteraction>()?.InstantiateAndDropCarrots();
        }

        yield return new WaitForSeconds(0.2f);
        DirtSpawnerPooling.DeSpawn(thisCropDirt);
        Destroy(gameObject.transform.parent.gameObject);
    }

    private Vector3 GetPosition()
    {
        return new Vector3(transform.parent.parent.position.x, 2, transform.parent.parent.position.z);
    }
}
