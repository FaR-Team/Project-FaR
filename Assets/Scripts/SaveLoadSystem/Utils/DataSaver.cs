using System.Threading.Tasks;
using UnityEngine;

public abstract class DataSaver<T, Y> : MonoBehaviour where Y : DataSaver<T, Y>, IDataSavable
{
    //Clase abstracta la cual debe heredar la clase que se encargue de guardar el objeto a guardar (T)
    protected T objectToSave;
    protected UniqueID uniqueiD;

    [SerializeField] protected ISaver saverAllData;

    [SerializeField] protected Y thisDataSaver;

    private void Awake()
    {
        uniqueiD = GetComponent<UniqueID>();
        objectToSave = GetComponent<T>();
    }
    protected abstract void SetThisInstance();

    private void OnEnable()
    {

        SetThisInstance();

        saverAllData.AddSavedObject(thisDataSaver);

    }

    private void OnDisable()
    {
        saverAllData.RemoveSavedObject(thisDataSaver);
    }

    public abstract Task SaveData();
}

public interface IDataSavable
{
    public Task SaveData();
}