using UnityEngine;

public abstract class Saver<T, Y> : MonoBehaviour
{
    Y data;
    public abstract void WriteSave(T info);
    
    public abstract void SaveAll(bool isTemporarySave);
}
