using UnityEngine;

public abstract class Saver<T> : MonoBehaviour
{
    public abstract void WriteSave(T info);
    
    public abstract void SaveAll(bool isTemporarySave);
}
