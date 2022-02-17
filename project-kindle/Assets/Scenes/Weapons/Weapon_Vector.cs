using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon_Vector : Weapon
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            AddEnergy(4);
        }
    }
}

public class Weapon_Vector_Lvl1 : WeaponLvl
{

}
