using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Meteor : Weapon
{
    public override void Fire(float dir)
    {
        ShootProjectile(GetLevelIndex(), dir);
    }
    
    protected override void OnShoot()
    {
        game.PlaySound("DragonShoot");
    }
}
