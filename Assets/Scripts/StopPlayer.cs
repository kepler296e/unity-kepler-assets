using UnityEngine;
using System.Collections.Generic;

public class StopPlayer : MonoBehaviour
{
    private Transform player;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject.transform;
            player.parent = transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.parent = null;
            player.rotation = Quaternion.identity;
        }
    }
}