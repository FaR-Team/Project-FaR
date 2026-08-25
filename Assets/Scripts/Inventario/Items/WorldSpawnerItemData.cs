using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/SpecialItem/WorldSpawnerItem", order = 0)]
public class WorldSpawnerItemData : SpecialItemData
{
    public GameObject prefabToSpawn;

    public Vector3 spawnPosition = Vector3.zero;

    public Vector3 spawnRotationEuler = Vector3.zero;

    public string spawnPointTag = "";

    public bool removeFromShopPool = true;

    public bool addToPlayerInventory = false;

    public virtual GameObject SpawnInWorld()
    {
        GameObject prefab = prefabToSpawn != null ? prefabToSpawn : ItemPrefab;
        if (prefab == null)
        {
            Debug.LogWarning($"[WorldSpawnerItemData] ({Nombre}): No hay prefabToSpawn ni ItemPrefab asignado.");
            return null;
        }

        Vector3 pos = spawnPosition;
        Quaternion rot = Quaternion.Euler(spawnRotationEuler);

        if (!string.IsNullOrEmpty(spawnPointTag))
        {
            GameObject anchor = GameObject.FindWithTag(spawnPointTag);
            if (anchor != null)
            {
                pos = anchor.transform.position;
                rot = anchor.transform.rotation;
            }
        }

        GameObject spawned = Instantiate(prefab, pos, rot);
        return spawned;
    }

    public virtual void OnPurchased(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnInWorld();
        }
    }

    public override bool UseItem()
    {
        return SpawnInWorld() != null;
    }
}
