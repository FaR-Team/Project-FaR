using System.Collections;
using UnityEngine;

public class SpawnerPooling : MonoBehaviour
{


    void Start()
    {
        //ObjectPooling.PreLoad(cube, 5);
    }

    // Update is called once per frame
    void Update()
    {
        SpawnObject();

    }

    private void SpawnObject()
    {
        /*Vector3 vector = SpawnPosition();
        GameObject c = ObjectPooling.GetObject(cube);
        c.transform.position = vector;
        StartCoroutine(DeSpawn(cube, c, 2.0f));*/
    }

    Vector3 SpawnPosition()
    {
        float x = Random.Range(-10.0f, 10.0f);
        float y = 0.5f;
        float z = Random.Range(-10.0f, 10.0f);

        Vector3 vector = new Vector3(x, y, z);

        return vector;
    }

    void DeSpawn(GameObject primitive, GameObject go)
    {
        //ObjectPooling.RecicleObject(primitive, go);
    }

}
