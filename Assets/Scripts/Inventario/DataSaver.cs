using System.Threading.Tasks;
using UnityEngine;

public abstract class DataSaver<T> : MonoBehaviour, IDataSaver
{
    protected T objectToSave;
    protected UniqueID uniqueiD;
    protected virtual void Awake()
    {
        uniqueiD = GetComponent<UniqueID>();
        objectToSave = GetComponent<T>();
    }
    protected virtual void Start()
    {
        SetObserver();
    }

    protected abstract void SetObserver();

    public abstract Task SaveData();
}