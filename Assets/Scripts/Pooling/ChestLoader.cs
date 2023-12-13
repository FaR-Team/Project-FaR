using UnityEngine;

public class ChestLoader : MonoBehaviour
{
    private AllChestSystems chestsDatas;
    [SerializeField] private GameObject chestPrefab;

    private void LoadChests()
    {
        chestsDatas = LoadAllChestData.GetData(false);
       
        if (chestsDatas == null) return;

        for(int i = 0; i < chestsDatas.dataCounter; i++)
        {
            GameObject chest = Instantiate(chestPrefab, gameObject.transform);
            chest.GetComponent<Cofre>().LoadData(chestsDatas.data[i]);
        }
    }
}
