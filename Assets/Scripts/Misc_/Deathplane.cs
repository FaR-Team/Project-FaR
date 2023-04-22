using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathplane : MonoBehaviour
{
    private Transform player;

    void Start() {
        player = this.gameObject.transform;
    }

    void Update()
    {
        if (player.position.y <= -50)
        {
            player.position = GameObject.Find("SpawnPoint").gameObject.transform.position;
        }
    }
}
