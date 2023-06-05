using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    bool _isEmpty;
    public bool _isWet;

    [SerializeField] float areaHarvestDelay;

    public int abilityLevelPlaceholder = 1;
    public bool IsEmpty => _isEmpty;

    public GameObject violeta;
    public GameObject currentCrop;
    public SeedItemData currentCropData;
    private WaitForSeconds delay;
    private int rand;

    void Start()
    {
        _isEmpty = true;
        delay = new WaitForSeconds(areaHarvestDelay);
    }

    Vector3 SizeOfBox(int level)
    {
        if (level is 1)
        {
            return new Vector3(5, 0.1f, 5);
        }
        else if (level is 2)
        {
            return new Vector3(9, 0.1f, 9);
        }
        else return Vector3.zero;
    }

    public void CreateBox()
    {
        var box = this.gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        StartCoroutine(AreaHarvest(box));
    }

    private IEnumerator AreaHarvest(BoxCollider box)
    {
        if (PlayerStats.Instance.AreaHarvestLevel == 1)
        {
            box.size = SizeOfBox(0);
            yield return delay;
            box.size = SizeOfBox(1);
            
        }
        else if(PlayerStats.Instance.AreaHarvestLevel == 2)
        {
            box.size = SizeOfBox(0);
            yield return delay;
            box.size = SizeOfBox(1);
            yield return delay;
            box.size = SizeOfBox(2);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag is "Violeta" && !(col.gameObject == violeta))
        {
            if (col.GetComponentInParent<Dirt>().currentCropData == this.currentCropData)
            {
                col.GetComponentInParent<Dirt>().currentCrop.GetComponentInChildren<IInteractable>().InteractOut();
            }
        }
    }

    public bool GetCrop(SeedItemData itemData)
    {
        _isEmpty = false;
        Quaternion rotation;
        
        switch(rand)
        {
            case 1:
                rotation = Quaternion.Euler(0, 90, 0);
            break;
            case 2:
                rotation = Quaternion.Euler(0, -90, 0);
            break;
            case 3:
                rotation = Quaternion.Euler(0, -180, 0);
            break;
            case 4:
                rotation = Quaternion.Euler(0, 180, 0);
            break;
            default:
                rotation = Quaternion.Euler(0, 0, 0);
            break;
        }
        GameObject instantiated = GameObject.Instantiate(itemData.DirtPrefab, transform.position, rotation, transform);
        currentCrop = instantiated;
        currentCropData = itemData;
        return (instantiated != null);
    }

    public void MakeRand()
    {
        rand = UnityEngine.Random.Range(1,4);
    }
}
