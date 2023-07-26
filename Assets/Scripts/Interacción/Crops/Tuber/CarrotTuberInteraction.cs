using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils;

public class CarrotTuberInteraction : CropInteraction
{
    public GrowingCrop GrowingCrop;
    public GameObject Prefab;

    public override void Awake()
    {
        base.Awake();

        GrowingCrop = GetComponent<GrowingCrop>();
       
    }

    public override IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        
        foreach (GameObject fruit in Fruits())
        {
            fruit.AddComponent<Outline>();
            fruit.GetComponent<FallingFruit>().FallTuber();
        }

        GetComponent<CropExplode>().Chau();
    }

    public override List<GameObject> Fruits()
    {
        List<GameObject> fruits = new List<GameObject>();

        int randNum = Random.Range(1, 5);

        for (int i = 0; i < randNum; i++)
        {
            fruits.Add(Instantiate(Prefab, new Vector3(this.transform.position.x, 2, this.transform.position.z), Quaternion.Euler(0, 0, 0)));
        }
        
        return fruits;
    }
}
