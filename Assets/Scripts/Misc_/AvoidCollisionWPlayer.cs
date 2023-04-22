using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCollisionWPlayer : MonoBehaviour
{
    public GameObject player;
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");     
        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
    }

    private void OnDestroy() 
    {
        player = GameObject.FindGameObjectWithTag("Player");     
        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), false);
    }
}
