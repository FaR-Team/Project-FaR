using UnityEngine;

[System.Serializable]
public class DebrisData : SaveData
{
    public int prefabIndex;
    public Vector3 position;
    public Quaternion rotation;

    public DebrisData(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        this.prefabIndex = prefabIndex;
        this.position = position;
        this.rotation = rotation;
    }

    public DebrisData()
    {
        this.prefabIndex = -1;
        this.position = Vector3.zero;
        this.rotation = Quaternion.identity;
    }
}
