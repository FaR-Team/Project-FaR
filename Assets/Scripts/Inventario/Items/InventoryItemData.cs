using System;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/Item", order = 0)]
[System.Serializable]
public class InventoryItemData : ScriptableObject 
{
    #region Basic Item Properties
    [Header("Basic Information")]
    public int ID = -1;
    public string Nombre;
    [TextArea(3, 10)]
    public string Descripción;
    public Sprite Icono;
    public int Valor;
    
    [Header("Prefabs & Visuals")]
    public GameObject ItemPrefab;
    public GameObject DirtPrefabGhost;
    public Mesh ghostMesh;
    public GameObject ToolGameObject;
    
    [Header("Grid & Placement")]
    [Tooltip("The size of the grid space required for this item")]
    public Vector3 RequiredGridSpace = new Vector3(2.1f, 0.1f, 2.1f);
    #endregion

    #region Item Behavior
    [Header("Item Behavior")]
    public bool Usable = true;
    public bool Sellable = true;
    public bool IsLookingAtStore;
    public bool leftClickUse;
    public AudioClip useItemSound;
    
    [Header("Item Type")]
    public TypeOfItem typeOfItem;
    
    public virtual ItemCategory Category 
    { 
        get 
        {
            return typeOfItem switch
            {
                TypeOfItem.Hoe or TypeOfItem.Axe or TypeOfItem.Bucket or TypeOfItem.Shovel => ItemCategory.Tool,
                TypeOfItem.CropSeed or TypeOfItem.TreeSeed => ItemCategory.Seed,
                TypeOfItem.Crop => ItemCategory.Crop,
                TypeOfItem.Special => ItemCategory.Special,
                _ => ItemCategory.Special
            };
        }
    }
    #endregion

    #region Virtual Methods
    public virtual bool SearchTool() 
    {
        this.LogError("aaaaaaaa");
        return false;
    }

    public virtual bool UseItem()
    {
        this.Log($"Usando {Nombre}");
        return true;
    }

    public virtual bool UseItem(Dirt dirt)
    {
        this.Log($"Usando {Nombre} en tierra");
        return true;
    }
    #endregion

    #region Type Checking Methods
    public bool IsTool()
    {
        return typeOfItem == TypeOfItem.Hoe || 
               typeOfItem == TypeOfItem.Axe || 
               typeOfItem == TypeOfItem.Bucket || 
               typeOfItem == TypeOfItem.Shovel;
    }

    public bool IsOfType(TypeOfItem itemType) => typeOfItem == itemType;
    
    public bool IsHoe() => IsOfType(TypeOfItem.Hoe);
    public bool IsAxe() => IsOfType(TypeOfItem.Axe);
    public bool IsBucket() => IsOfType(TypeOfItem.Bucket);
    public bool IsShovel() => IsOfType(TypeOfItem.Shovel);
    public bool IsCropSeed() => IsOfType(TypeOfItem.CropSeed);
    public bool IsTreeSeed() => IsOfType(TypeOfItem.TreeSeed);
    public bool IsSpecialItem() => IsOfType(TypeOfItem.Special);
    #endregion
}