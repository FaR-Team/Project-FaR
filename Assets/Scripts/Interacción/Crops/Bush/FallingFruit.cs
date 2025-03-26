using UnityEngine;
using System.Collections;
using FaRUtils;

public class FallingFruit : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float destroyYThreshold = 1f; 
    private MaterialPropertyBlock _propertyBlock;
    // Esto seria tirar las frutas al piso y permitir al jugador recogerlas.

    private bool isFalling = false;
    private Rigidbody rb;

    void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
    }

    public void FallFruit()
    {
        StartCoroutine(DropFruit());
    }

    public void FallTuber()
    {
        StartCoroutine(LaunchTuber());
    }

    IEnumerator LaunchTuber()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);

        GetComponent<Rigidbody>().isKinematic = false;
        Vector3 force = transform.forward;
        force = new Vector3(force.x, 1, force.z);
        GetComponent<Rigidbody>().AddForce(force * speed, ForceMode.Impulse);

        yield return new WaitForSeconds(0.1f);

        GetComponent<ItemPickUp>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            _propertyBlock.SetFloat("_UseOutline", 0);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    private void Update()
    {
        if (isFalling && transform.position.y <= destroyYThreshold)
        {
            ItemPickUp itemPickUp = GetComponent<ItemPickUp>();
            if (itemPickUp != null && itemPickUp.ItemData != null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    var inventory = player.GetComponent<Container>();
                    if (inventory != null)
                    {
                        if (inventory.PrimaryInventorySystem.AddToInventory(itemPickUp.ItemData, 1))
                        {
                            if (MusicManager.Instance != null && itemPickUp.PickUpClip != null)
                            {
                                MusicManager.Instance.PlaySFX(itemPickUp.PickUpClip, itemPickUp.PickupVolume, 0.8f, 1.2f);
                            }
                            
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }

    IEnumerator DropFruit()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            _propertyBlock.SetFloat("_UseOutline", 1);
            renderer.SetPropertyBlock(_propertyBlock);
        }
        
        yield return new WaitForSeconds(0.5f);
        GetComponent<ItemPickUp>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        isFalling = true;
        
        if (renderer != null)
        {
            _propertyBlock.SetFloat("_UseOutline", 0);
            renderer.SetPropertyBlock(_propertyBlock);
        }
        
        gameObject.layer = 0;
        this.transform.parent = null;
    }
}