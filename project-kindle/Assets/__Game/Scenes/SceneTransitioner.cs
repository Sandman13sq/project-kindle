using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public void LoadNextLevel()
    {
       StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
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
