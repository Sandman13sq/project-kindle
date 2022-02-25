using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRestart : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Restart scene
        if (Input.GetKeyDown("r")) 
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("sc_prototype"); // Load scene called Game
        }

        // Exit Game
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
    }
}
