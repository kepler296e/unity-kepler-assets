using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Reset : MonoBehaviour
{
    public KeyCode resetKeyCode = KeyCode.F1;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(resetKeyCode))
            SceneManager.LoadScene("Playground");
    }
}