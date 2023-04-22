using System.Collections;
using UnityEngine;

public class SpawnerPooling : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        //ObjectPooling.PreLoad(cube, 5);
    }

    // Update is called once per frame
    void Update()
    {

        /*      Vector3 vector = SpawnPosition();
                GameObject c = ObjectPooling.GetObject(cube);
                c.transform.position = vector;
                StartCoroutine(DeSpawn(cube, c, 2.0f)); */

    }


    Vector3 SpawnPosition()
    {
        float x = Random.Range(-10.0f, 10.0f);
        float y = 0.5f;
        float z = Random.Range(-10.0f, 10.0f);

        Vector3 vector = new Vector3(x, y, z);

        return vector;
    }

    IEnumerator DeSpawn(GameObject primitive, GameObject go, float time)
    {

        yield return new WaitForSeconds(time);
        ObjectPooling.RecicleObject(primitive, go);

    }

}
