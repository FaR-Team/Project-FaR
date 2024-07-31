using System.Threading.Tasks;
using UnityEngine;

public abstract class Saver<T, Y> : MonoBehaviour, ISaver where T : SaveData where Y : IDataSavable
{
    protected void Start()
    {
        SaveLoadHandlerSystem.AddListener(SaveAllData);
    }

    public abstract Task WriteSave(T t);

    protected abstract void SaveAllData(bool isTemporarySave);

    public abstract void AddSavedObject(Y y);

    public abstract void RemoveSavedObject(Y y);


    Task ISaver.WriteSave(SaveData data) => WriteSave((T)data);
    void ISaver.AddSavedObject(IDataSavable savable) => AddSavedObject((Y)savable);
    void ISaver.RemoveSavedObject(IDataSavable savable) => RemoveSavedObject((Y)savable);

}

public interface ISaver
{
    Task WriteSave(SaveData data);
    void AddSavedObject(IDataSavable savable);
    void RemoveSavedObject(IDataSavable savable);
}