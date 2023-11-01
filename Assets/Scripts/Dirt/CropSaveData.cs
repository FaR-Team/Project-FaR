[System.Serializable]
public class CropSaveData : SaveData
{
    public int DiasPlantado;
    public GrowingState GrowingState;
    public CropSaveData(int diasPlantado, GrowingState growingState)
    {
        DiasPlantado = diasPlantado;
        GrowingState = growingState;
    }
}
