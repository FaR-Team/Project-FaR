[System.Serializable]
public class CropSaveData : PlantData
{
    public int DiasPlantado;
    public int DaysDry;
    public int DaysWithoutHarvest;
    public GrowingState GrowingState;
    public bool isDead;

    public CropSaveData(GrowingBase crop)
    {
        DiasPlantado = crop.DaysPlanted;
        DaysDry = crop.DaysDry;
        DaysWithoutHarvest = crop.DaysWithoutHarvest;
        GrowingState = crop.currentState;
        isDead = crop.IsDead;
    }
}
