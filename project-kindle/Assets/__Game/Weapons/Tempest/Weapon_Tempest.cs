using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Tempest : Weapon
{
    public override void Fire(float dir)
    {
        // Small random direction
        ShootProjectile(GetLevelIndex(), dir + UnityEngine.Random.Range(-0.1f, 0.1f));
    }
    
    protected override void OnShoot()
    {
        game.PlaySound("DragonShoot");
    }
}
