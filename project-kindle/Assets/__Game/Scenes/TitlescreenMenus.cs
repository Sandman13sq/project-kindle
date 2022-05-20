using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlescreenMenus : MasterObject
{
    public Animator transition;
    public Animator background;
    public float transitionTime = 1f;

    public void StartGame()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Has Quit!");
    }

    public void OptionsMenu()
    {
        background.Play("Base Layer.OptionsBackground", 0, 0.0f);
    }

    public void BackButton()
    {
        background.Play("Base Layer.MenuBackground", 0, 0.0f);
    }

    //Use coroutine to play transition animations before loading level!
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        //wait for animation!
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
