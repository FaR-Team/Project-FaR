using UnityEngine;

[System.Serializable]
public class DirtData : SaveData
{
    public bool _isWet;
    public bool IsEmpty;
    public int currentCropID;
    public CropSaveData cropSaveData;
    public Vector3 position;

    public DirtData(bool isWet, bool isEmpty, int currentCropID, CropSaveData cropSaveData, Vector3 position)
    {
        _isWet = isWet;
        IsEmpty = isEmpty;
        this.currentCropID = currentCropID;
        this.cropSaveData = cropSaveData;
        this.position = position;
    }
    public DirtData()
    {
        _isWet = false;
        IsEmpty = true;
        this.currentCropID = -1;
        this.cropSaveData = null;
    }
}
