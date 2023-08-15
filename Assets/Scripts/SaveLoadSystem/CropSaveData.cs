using UnityEngine;

[System.Serializable]
public struct CropSaveData
{
    public Vector3 Position;
    public int Dia;
    public string ID;

    public CropSaveData(int _dia, Vector3 _position, string _id)
    {
        Dia = _dia;
        Position = _position;
        ID = _id;
    }
}
