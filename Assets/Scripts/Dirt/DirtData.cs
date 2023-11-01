using UnityEngine;

[System.Serializable]
public class DirtData : SaveData
{
    public bool _isWet;
    public bool IsEmpty;
    public GameObject currentCrop;
    public SeedItemData currentCropData;
    public CropSaveData cropSaveData;
    public Vector3 position;

    public DirtData(bool isWet, bool isEmpty, GameObject currentCrop, SeedItemData currentCropData, CropSaveData cropSaveData, Vector3 position)
    {
        _isWet = isWet;
        IsEmpty = isEmpty;
        this.currentCrop = currentCrop;
        this.currentCropData = currentCropData;
        this.cropSaveData = cropSaveData;
        this.position = position;
    }
    public DirtData()
    {
        _isWet = false;
        IsEmpty = true;
        this.currentCrop = null;
        this.currentCropData = null;
        this.cropSaveData = null;
    }
}
