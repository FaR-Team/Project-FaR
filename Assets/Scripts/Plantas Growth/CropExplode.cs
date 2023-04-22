using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class CropExplode : MonoBehaviour
{   
    public string _objectName;
    public GameObject Tierra = null;
    public GameObject Coso;
    public GameObject Parent;
    //bool YaExploto = false;
    public InventoryItemData ItemData;
    public GameObject jugador;
    public GameObject Colliders;
    public GameObject Crop;

    public Transform tierraAnim;
    //public GameObject CropLeaf;
    public Vector3 center;
    public float radius = 10;


    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        Tierra = transform.root.GetChild(0).gameObject;
        center = new Vector3(transform.root.GetChild(0).position.x, transform.root.GetChild(0).position.y, transform.root.GetChild(0).position.z);
        Parent = transform.root.gameObject;
    }    
    
    void LateUpdate() 
    {
        var distance = Vector3.Distance(this.gameObject.transform.position, Tierra.transform.position);
    }

    public void Chau()
    {
        Tierra = transform.root.GetChild(0).gameObject;
        Vector3 pos = new Vector3 (transform.root.GetChild(0).position.x, 2, transform.root.GetChild(0).position.z);
        var inventory = jugador.transform.GetComponent<PlayerInventoryHolder>();

        //Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        //OnDrawGizmos();

        /*if (hitColliders  != null)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.tag == _objectName && hitCollider != this.gameObject.GetComponent<Collider>())
                {
                    //hitCollider.gameObject.GetComponent<Carrot>().InteractOut();
                    //TODO: No matarse (En swahilli).
                }
            }
        }*/
        var rand = Random.Range(1,5);
        inventory.AñadirAInventario(ItemData, rand);

        //GameObject boom = Instantiate(Coso, pos, Quaternion.Euler(0,0,0));
        //YaExploto = true;
        StartCoroutine(Destruir());
    }

    //private void OnDrawGizmos()
    //{
        //Gizmos.DrawWireSphere(center, radius);
    //}
    IEnumerator Destruir()
    {
        Vector3 pos = new Vector3 (transform.root.GetChild(0).position.x, 2, transform.root.GetChild(0).position.z);
        for (int i = 0; i < Tierra.gameObject.transform.childCount; i++)
        {
            if(Tierra.gameObject.transform.GetChild(i).gameObject.activeSelf == true && Tierra.gameObject.transform.GetChild(i).gameObject.GetComponent<Animation>() != null)
            {
                tierraAnim = Tierra.gameObject.transform.GetChild(i);
            }
        }
        
        yield return new WaitForSeconds(2.5f);
        Tierra.GetComponent<Animation>().Play();
        GameObject boom = Instantiate(Coso, pos, Quaternion.Euler(0,0,0));
        Crop.GetComponent<SkinnedMeshRenderer>().enabled = false;
        Destroy(Crop.gameObject.GetComponent<Outline>());
        yield return new WaitForSeconds(0.5f);
        Destroy(Tierra);
        Destroy(Parent);
    }
}
