using UnityEngine;
using System.Collections.Generic;

public class WorldUI : MonoBehaviour
{
    private bool playerClose;

    private void Update()
    {
        if (playerClose)
        {
            GetComponent<Canvas>().enabled = true;
            transform.LookAt(Camera.main.transform);
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") playerClose = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") playerClose = false;
    }
}