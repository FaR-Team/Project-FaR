using System.Collections;
using UnityEngine;

public class DirtAreaHarvest : MonoBehaviour
{
    private WaitForSeconds delay;

    [Range(0.01f, 3f)]
    [SerializeField] float areaHarvestDelay;

    Dirt dirt;

    private void Start()
    {
        delay = new WaitForSeconds(areaHarvestDelay);
        dirt = GetComponent<Dirt>();
    }
    public void CreateAreaForHarvest() //This Creates the box collider that would harvest in an area.
    {
        var box = this.gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        StartCoroutine(AreaHarvest(box));
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


    private IEnumerator AreaHarvest(BoxCollider box)
    {
        if (PlayerStats.Instance.AreaHarvestLevel == 1)
        {
            box.size = SizeOfBox(0);
            yield return delay;
            box.size = SizeOfBox(1);

        }
        else if (PlayerStats.Instance.AreaHarvestLevel == 2)
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
        if (col.tag is "Violeta" && !(col.gameObject == dirt.violeta))
        {
            if (col.GetComponentInParent<Dirt>().currentCropData == dirt.currentCropData)
            {
                col.GetComponentInParent<Dirt>().currentCrop.GetComponentInChildren<IInteractable>().InteractOut();
            }
        }
    }
}
