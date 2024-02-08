using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<string> Items;
    public SerializableDictionary<string, ItemPickUpSaveData> activeItems;

    public SerializableDictionary<string, InventorySaveData> chestDictionary;

    public SerializableDictionary<string, CropSaveData> cropDictionary;

    public InventorySaveData playerInventory;

    public SaveData()
    {
        Items = new List<string>();
        activeItems = new SerializableDictionary<string, ItemPickUpSaveData>();
        cropDictionary = new SerializableDictionary<string, CropSaveData>();
        chestDictionary = new SerializableDictionary<string, InventorySaveData>();
        playerInventory = new InventorySaveData();
    }
}