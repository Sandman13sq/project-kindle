using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenu : MasterObject
{
    public Canvas mainMenu;
    public Animator anim;
    void Start()
    {
        mainMenu.enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            mainMenu.enabled = true;
            anim.Play("Base Layer.MenuBackground", 0, 0.0f);
        }
    }
}
