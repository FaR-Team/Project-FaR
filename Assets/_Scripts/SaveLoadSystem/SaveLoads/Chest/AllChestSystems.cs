using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AllChestSystems : IAllData<AllChestSystems>
{
    public List<ChestData> dataList;
    public Queue<ChestData> data;
    public int dataCounter;
    
    public AllChestSystems()
    {
        dataCounter = 0;
        dataList = new List<ChestData>();
        data = new Queue<ChestData>();
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

    public void CopyData(AllChestSystems allChestsData)
    {
        dataList = allChestsData.dataList;
        dataCounter = allChestsData.dataCounter;
        LoadQueue();
    }
    
    public void ClearAfterSave()
    {
        dataCounter = 0;
        data.Clear();
    }
}