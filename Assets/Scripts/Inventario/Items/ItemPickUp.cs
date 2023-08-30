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
        SaveLoad.OnLoadGame += LoadGame;
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
        SaveGameManager.data.activeItems.Add(id, itemSaveData);
    }

    private void LoadGame(SaveData data)
    {
        if (data.activeItems.ContainsKey(id))
        {
            //Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (SaveGameManager.data.activeItems.ContainsKey(id))
        {
            SaveGameManager.data.activeItems.Remove(id);
            SaveLoad.OnLoadGame -= LoadGame;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<InventoryHolder>();

        if (!inventory) return;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.volume = 0.3f;
        
        audioSource.PlayOneShot(PickUpClip);

        if (inventory.PrimaryInventorySystem.AñadirAInventario(ItemData, 1))
        {
            SaveGameManager.data.Items.Add(id);
            Destroy(this.gameObject);
        }
        
    }

    public static void GiveItem(InventoryItemData data, int amount)
    {
        var inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryHolder>();

        if (!inventory)
        {
            return;
        }
        inventory.PrimaryInventorySystem.AñadirAInventario(data, amount);
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
