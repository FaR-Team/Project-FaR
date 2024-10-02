using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathplane : MonoBehaviour
{
    private Transform player;

    void Start() {
        player = this.gameObject.transform;
    }

    void Update() // TODO: Sacar esto, de ultima usar un trigger abajo
    {
        if (player.position.y <= -50)
        {
            player.position = FindObjectOfType<SpawnPoint>().gameObject.transform.position;
        }
    }
}
