using UnityEngine;

[System.Serializable]
public class DirtData : SaveData
{
    public bool _isWet;
    public bool IsEmpty;
    public SeedItemData currentCropData;
    [SerializeReference] public PlantData plantData; // SerializeReference attribute allows polymorphism in custom serializable classes
    public Vector3Int coordinate;

    public DirtData(bool isWet, bool isEmpty, SeedItemData currentCropData, PlantData plantSaveData, Vector3Int coordinate)
    {
        _isWet = isWet;
        IsEmpty = isEmpty;
        this.currentCropData = currentCropData;
        plantData = plantSaveData;
        this.coordinate = coordinate;
    }
    public DirtData()
    {
        _isWet = false;
        IsEmpty = true;
        this.currentCropData = null;
        this.plantData = null;
    }
}
