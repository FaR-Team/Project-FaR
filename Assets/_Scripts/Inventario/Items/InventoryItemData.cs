using System;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/Item", order = 0)]


[System.Serializable]

public class InventoryItemData : ScriptableObject 
{   
    public int ID = -1;
    public string Nombre;
    [TextArea(3,10)]
    public string Descripción;
    public Sprite Icono;
    public int Valor;
    public GameObject ItemPrefab;

    public GameObject DirtPrefabGhost = null;
    public Mesh ghostMesh = null;
    public GameObject ToolGameObject = null;

    public bool Usable;
    public bool Sellable;
    public bool IsLookingAtStore;
    public bool leftClickUse = false;
    public AudioClip useItemSound;

    public TypeOfItem typeOfItem;

    public virtual bool SearchTool() 
    {
        this.LogError("aaaaaaaaaa");
        return false;
    }

    public virtual bool UseItem()
    {
        this.Log($"Usando {Nombre}");
        return true;
    }

    public virtual bool UseItem(Dirt dirt)
    {
        this.Log($"Usando {Nombre}");
        return true;
    }
    public bool IsTool()
    {
        return TypeOfItem.Hoe == typeOfItem || TypeOfItem.Hoe == typeOfItem || TypeOfItem.Axe == typeOfItem || TypeOfItem.Bucket == typeOfItem || TypeOfItem.Shovel == typeOfItem;
    }
    public bool IsHoe()
    {
        return TypeOfItem.Hoe == typeOfItem;
    }
    public bool IsAxe()
    {
        return TypeOfItem.Axe == typeOfItem;
    }
    public bool IsBucket()
    {
        return TypeOfItem.Bucket == typeOfItem;
    }
    public bool IsShovel()
    {
        return TypeOfItem.Shovel == typeOfItem;
    }
    public bool IsCropSeed()
    {
        return TypeOfItem.CropSeed == typeOfItem;
    }
    public bool IsTreeSeed()
    {
        return TypeOfItem.TreeSeed == typeOfItem;
    }
    
    public bool IsSpecialItem()
    {
        return TypeOfItem.Special == typeOfItem;
    }
}