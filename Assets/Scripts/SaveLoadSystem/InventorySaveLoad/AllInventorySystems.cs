
using System;
using System.Collections.Generic;

[System.Serializable]
public class AllInventorySystems
{
    public int dataCounter;
    public InvDictionary data;

    public AllInventorySystems(int dataCounter)
    {
        this.dataCounter = dataCounter;
        data = new InvDictionary();
    }
    public AllInventorySystems()
    {
        dataCounter = 0;
        data = new InvDictionary();
    }

    public AllInventorySystems(InvDictionary data, int dataCounter)
    {
        this.dataCounter = dataCounter;
        this.data = data;
    }
}

[Serializable]
public class InvDictionary : SerializableDictionary<string, InventorySystem> { }