using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistent<T>
{
    public T SaveData { get; }

    void Init();
    void Save();
    void Load(SaveData data);
}