using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MasterObject
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("c") && Input.GetKeyDown("return"))
        {
            if(isPaused)
            {
                Resume();
                game.GameFlagClear(_GameHeader.GameFlag.pause);
                game.GameFlagClear(_GameHeader.GameFlag.lock_player);
            }
            else
            {
                Pause();
                game.GameFlagSet(_GameHeader.GameFlag.lock_player);
                game.GameFlagSet(_GameHeader.GameFlag.pause);
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
}
