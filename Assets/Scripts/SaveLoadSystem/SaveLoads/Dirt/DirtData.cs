using UnityEngine;

[System.Serializable]
public class DirtData : SaveData
{
    public bool _isWet;
    public bool IsEmpty;
    public SeedItemData currentCropData;
    public CropSaveData cropSaveData;
    public Vector3 position;

    public DirtData(bool isWet, bool isEmpty, SeedItemData currentCropData, CropSaveData cropSaveData, Vector3 position)
    {
        _isWet = isWet;
        IsEmpty = isEmpty;
        this.currentCropData = currentCropData;
        this.cropSaveData = cropSaveData;
        this.position = position;
    }
    public DirtData()
    {
        _isWet = false;
        IsEmpty = true;
        this.currentCropData = null;
        this.cropSaveData = null;
    }
}
