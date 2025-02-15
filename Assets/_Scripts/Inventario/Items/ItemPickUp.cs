using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(UniqueID))]
public class ItemPickUp : MonoBehaviour
{
    public float PickUpRadius = 1f;
    public InventoryItemData ItemData;
    public AudioClip PickUpClip;

    private SphereCollider _collider;
    private AudioSource audioSource;
    private GameObject player;

    [SerializeField] private ItemPickUpSaveData itemSaveData;
    private string id;

    private void Awake()
    {
        itemSaveData = new ItemPickUpSaveData(ItemData, transform.position, transform.rotation);
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = player.GetComponent<AudioSource>();

        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _collider.radius = PickUpRadius;
    }

    private void Start()
    {
        id = GetComponent<UniqueID>().ID;
    }

    private void LoadGame(SaveData data)
    {
      
    }

    private void OnDestroy()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<Container>();

        if (!inventory) return;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.volume = 0.05f;
        
        audioSource.PlayOneShot(PickUpClip);

        if (inventory.PrimaryInventorySystem.AddToInventory(ItemData, 1))
        {
            Destroy(this.gameObject);
        }
        
    }

    public static void GiveItem(InventoryItemData data, int amount)
    {
        var inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Container>();

        if (!inventory) return;
     
        inventory.PrimaryInventorySystem.AddToInventory(data, amount);
    }
}

[System.Serializable]
public struct ItemPickUpSaveData
{
    public InventoryItemData ItemData;
    public Vector3 Position;
    public Quaternion Rotation;

    public ItemPickUpSaveData(InventoryItemData _itemData, Vector3 _position, Quaternion _rotation)
    {
        ItemData = _itemData;
        Position = _position;
        Rotation = _rotation;
    }
}
