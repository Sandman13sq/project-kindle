using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlescreenMenus : MasterObject
{
    public UnityEngine.UI.Button startButton;
    public UnityEngine.UI.Button optionsButton;
    public UnityEngine.UI.Button quitButton;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public Animator transition;
    public Animator background;
    public float transitionTime = 1f;
    private int currButton = -1; //0 = start, 1 = options, 2 = quit
    bool leftHolding = false;
    bool rightHolding = false;
    bool upHolding = false;
    bool downHolding = false;
    bool controllerActive = false;
    private void Update() 
    {
        //input vars
        float xaxis = Input.GetAxisRaw("Horizontal");
        float yaxis = Input.GetAxisRaw("Vertical");

        //when the player stops holding a direction, reset the holding variable
        if (xaxis > -0.5)
            leftHolding = false;
        if (xaxis < 0.5)
            rightHolding = false;
        if (yaxis > -0.5)
            downHolding = false;
        if (yaxis < 0.5)
            upHolding = false;

        //only highlight something when controller is being used
        if( (xaxis != 0 || yaxis != 0) && controllerActive == false )
        {
            controllerActive = true;
            currButton = 0;
        }

        //left
        if(xaxis < -0.5 && leftHolding == false)
        {
            leftHolding = true;
            if(currButton == 1)
                currButton = 0;
            else if(currButton == 2)
                currButton = 1;
        }

        //right
        if(xaxis > 0.5 && rightHolding == false)
        {
            rightHolding = true;
            if(currButton == 0)
                currButton = 1;
            else if(currButton == 1)
                currButton = 2;
        }

        if(currButton == 0)
        {
            startButton.Select();
            if(Input.GetButton("Jump"))
                StartGame();
        }

        else if(currButton == 1)
        {
            optionsButton.Select();
            if(Input.GetButton("Jump"))
            {
                mainMenu.SetActive(false);
                optionsMenu.SetActive(true);
                OptionsMenu();
            }
        }

        else if(currButton == 2)
        {
            quitButton.Select();
            if(Input.GetButton("Jump"))
                QuitGame();
        }
    }

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
