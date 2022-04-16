using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon_Vector : Weapon
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown("e"))
        {
            AddEnergy(3);
        }

        if (Input.GetKeyDown("r"))
        {
            AddEnergy(-7);
        }
    }

    protected override void OnShoot()
    {
        game.PlaySound("VectorShoot");
    }    
}

