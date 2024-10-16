using UnityEngine.Serialization;

[System.Serializable]
public class CropSaveData : SaveData
{
    public int DiasPlantado;
    public int GrowingStateID;
    public CropSaveData(int diasPlantado, int growingStateID)
    {
        DiasPlantado = diasPlantado;
        GrowingStateID = growingStateID;
    }
}
