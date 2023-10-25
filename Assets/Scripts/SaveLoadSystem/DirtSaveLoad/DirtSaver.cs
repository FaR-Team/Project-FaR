public class DirtSaver : Saver<DirtSaveData>
{
    public static DirtSaver instance;

    private AllDirtsData dirtsData;

    private void Awake()
    {
        dirtsData = new AllDirtsData();
    }

    public void WriteSave(DirtSaveData info, bool _isTemporarySave)
    {
        dirtsData.data.Enqueue(info);
        dirtsData.DirtCounter++;
        SaveAll(_isTemporarySave);
    }

    public override void SaveAll(bool isTemporarySave) //no se cuando ni como llamarlo
    {
        if (dirtsData.data.Count != DirtSpawnerPooling.GetActiveDirtPool()) return;
        SaverManager.Save(dirtsData, isTemporarySave);
    }

    public override void WriteSave(DirtSaveData info)
    {
        throw new System.NotImplementedException();
    }
} 
