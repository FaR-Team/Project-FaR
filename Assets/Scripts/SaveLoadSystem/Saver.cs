using UnityEngine;
using System.Threading.Tasks;

public abstract class Saver<T, Y> : MonoBehaviour
{
    protected void Start()
    {
        Cama.Instance.SaveDataEvent.AddListener(SaveAllData);
    }

    public abstract Task WriteSave(T t);

    protected abstract void SaveAllData(bool isTemporarySave);

    public abstract void AddSavedObject(Y y);

    public abstract void RemoveSavedObject(Y y);
}