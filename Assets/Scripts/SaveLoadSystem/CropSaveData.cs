using UnityEngine;

[System.Serializable]
public struct CropSaveData
{
    public Vector3 Position;
    public int _Dia;

    public CropSaveData(int Dia, Vector3 _position)
    {
        _Dia = Dia;
        Position = _position;
    }
}
