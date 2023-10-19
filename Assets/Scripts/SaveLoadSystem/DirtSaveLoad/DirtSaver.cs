public class DirtSaver : Saver<DirtSaveData>
{
    public static DirtSaver instance;

    private AllDirtsData dirtsData;

    private void Awake()
    {
        dirtsData = new AllDirtsData();
    }

    public override void WriteSave(DirtSaveData info)
    {
        dirtsData.data.Enqueue(info);
        dirtsData.DirtCounter++;
    }

    public override void SaveAll(bool isTemporarySave) //no se cuando ni como llamarlo
    {
        SaverManager.Save(dirtsData, isTemporarySave);
    }
} 
