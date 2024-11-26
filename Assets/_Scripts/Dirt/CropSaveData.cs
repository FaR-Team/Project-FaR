[System.Serializable]
public class CropSaveData
{
    public int DiasPlantado;
    public int DaysDry;
    public int DaysWithoutHarvest;
    public GrowingState GrowingState;

    public CropSaveData(GrowingBase crop)
    {
        DiasPlantado = crop.DaysPlanted;
        DaysDry = crop.DaysDry;
        DaysWithoutHarvest = crop.DaysWithoutHarvest;
        GrowingState = crop.currentState;
    }
}
