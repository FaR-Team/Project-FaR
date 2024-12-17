using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils;

public class CarrotTuberInteraction : CropInteraction
{
    public GrowingCrop GrowingCrop;
    public GameObject Prefab;
    private MaterialPropertyBlock _propertyBlock;


    public override void Awake()
    {
        base.Awake();
        _propertyBlock = new MaterialPropertyBlock();
        GrowingCrop = GetComponent<GrowingCrop>();
    }

    public override void Harvest()
    {
        var VisibleGO = this.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
        var renderer = VisibleGO.GetComponent<Renderer>();
        if (renderer != null)
        {
            _propertyBlock.SetFloat("_UseOutline", 1);
            renderer.SetPropertyBlock(_propertyBlock);
        }
        this.GetComponent<CropExplode>().StartAnimationAndExplode();
    }

    public void InstantiateAndDropCarrots()
    {
        List<GameObject> Fruits = this.Fruits();
        
        foreach (GameObject fruit in Fruits)
        {
            var renderer = fruit.GetComponent<Renderer>();
            if (renderer != null)
            {
                _propertyBlock.SetFloat("_UseOutline", 1);
                renderer.SetPropertyBlock(_propertyBlock);
            }
            fruit.GetComponent<FallingFruit>().FallTuber();
        }
    }

    public override List<GameObject> Fruits()
    {
        List<GameObject> fruits = new List<GameObject>();

        int randNum = Random.Range(1, 5);

        for (int i = 0; i <= randNum; i++)
        {
            fruits.Add(Instantiate(Prefab, new Vector3(this.transform.position.x, 2, this.transform.position.z), Quaternion.identity));
        }
        
        return fruits;
    }
}