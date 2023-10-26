using UnityEngine;

[System.Serializable]
public class DirtSaveData : SaveData
{
    public bool _isWet;
    public bool IsEmpty;
    public GameObject currentCrop;
    public SeedItemData currentCropData;
    public CropSaveData cropSaveData;

    public DirtSaveData(bool isWet, bool isEmpty, GameObject currentCrop, SeedItemData currentCropData, CropSaveData cropSaveData)
    {
        _isWet = isWet;
        IsEmpty = isEmpty;
        this.currentCrop = currentCrop;
        this.currentCropData = currentCropData;
        this.cropSaveData = cropSaveData;
    }
    public DirtSaveData()
    {
        _isWet = false;
        IsEmpty = true;
        this.currentCrop = null;
        this.currentCropData = null;
        this.cropSaveData = null;
    }
}
