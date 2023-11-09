using System.Collections.Generic;

[System.Serializable]
public class AllInventorySystems
{
    public List<InventorySystem> dataListValues;
    public List<string> keys;
    public int dataCounter;
    public Dictionary<string, InventorySystem> data;

    public AllInventorySystems(int dataCounter)
    {
        dataListValues = new List<InventorySystem>();
        keys = new List<string>();
        data = new Dictionary<string, InventorySystem>();
        this.dataCounter = dataCounter;
    }

    public AllInventorySystems(List<InventorySystem> dataListValues, List<string> keys, int dataCounter)
    {
        this.dataListValues = dataListValues;
        this.dataCounter = dataCounter;
        this.keys = keys;

        data = new Dictionary<string, InventorySystem>();
        LoadDict();
    }

    public void SaveDict()
    {
        foreach (var par in data)
        {
            keys.Add(par.Key);
            dataListValues.Add(par.Value);
        }
    }

    public void LoadDict()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            data.Add(keys[i], dataListValues[i]);
        }
    }
}