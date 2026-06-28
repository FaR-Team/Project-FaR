using UnityEngine;
using System.Threading.Tasks;
using FaRUtils.Systems.Debris;

[RequireComponent(typeof(Debris))]
public class SaveDebrisData : DataSaver<Debris, SaveDebrisData>, IDataSavable
{
    [HideInInspector] public int prefabIndex = -1;

    protected override void SetThisInstance()
    {
        thisDataSaver = this;
        saverAllData = DebrisSaver.instance;
    }   
    
    public override async Task SaveData()
    {
        DebrisData debrisSaveData = new DebrisData(prefabIndex, transform.position, transform.rotation);
        await saverAllData.WriteSave(debrisSaveData);
    }
}
