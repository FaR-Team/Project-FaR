using UnityEngine;

[System.Serializable]
public class DirtSaveData : SaveData
{
    public bool _isWet;
    public bool IsEmpty;
    public GameObject currentCrop;
    public SeedItemData currentCropData;
    public CropSaveData cropSaveData;
}
