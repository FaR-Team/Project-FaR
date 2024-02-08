using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class CropExplodeTree : MonoBehaviour
{
    public GameObject Tierra = null;
    public GameObject Coso;
    public GameObject Parent;
    //bool YaExploto = false;
    public InventoryItemData ItemData;
    public GameObject jugador;
    public GameObject Colliders;
    public GameObject Crop;
    public GameObject CropLeaf;


    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        Tierra = this.transform.root.GetChild(0).gameObject;
    }
    

    public void Chau(GameObject CropObj)
    {
        Tierra = this.transform.root.GetChild(0).gameObject;
        Vector3 pos = new Vector3 (transform.root.GetChild(0).position.x, 2, transform.root.GetChild(0).position.z);
        var inventory = jugador.transform.GetComponent<PlayerInventoryHolder>();
        inventory.AñadirAInventario(ItemData, 1);
        GameObject boom = Instantiate(Coso, pos, Quaternion.Euler(0,0,0));
        //YaExploto = true;
        Crop = CropObj;
        StartCoroutine(Destruir());
    }
    
    IEnumerator Destruir()
    {
        //CropLeaf.GetComponent<MeshRenderer>().enabled = false;
        Crop.GetComponent<MeshRenderer>().enabled = false;
        Destroy(Crop.gameObject.GetComponent<Outline>());
        yield return new WaitForSeconds(0.5f);
        Destroy(Parent);
    }
}
