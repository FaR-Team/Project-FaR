using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AllDirtsData
{
    public List<DirtSaveData> dataList;
    public Queue<DirtSaveData> data;
    public int DirtCounter;

    public AllDirtsData(int dirtCounter)
    {
        dataList = new List<DirtSaveData>();
        data = new Queue<DirtSaveData>();
        DirtCounter = dirtCounter;
    }

    public void SaveQueue()
    {
        dataList = data.ToList();
    }
    public void LoadQueue()
    {
        foreach (var item in dataList)
        {
            data.Enqueue(item);
        }
    }
}
