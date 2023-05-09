using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esto es un objeto scriptable object que define qué es un item en el juego.
/// Se podría reutilizar para otro tipo de objetos, uwu.
/// </summary>


[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/Item", order = 0)]

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
    public GameObject ToolGameObject = null;

    public bool Usable;
    public bool Sellable;
    public bool Seed;
    public bool TreeSeed;
    public bool IsLookingAtStore;


[Header("ToolBool")]
    public bool Tool;
    public bool IsHoe, IsAxe, IsBucket, IsUnknown1, IsUnknown2;
    //public int CantidadMáxima;

    public virtual bool SearchTool() 
    {
        Debug.LogError("aaaaaaaaaa");
        return false;
    }

    public virtual bool UseItem()
    {
        Debug.Log($"Usando {Nombre}");
        return true;
    }

    public virtual bool UseItem(Dirt dirt)
    {
        Debug.Log($"Usando {Nombre}");
        return true;
    }
}

