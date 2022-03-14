using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Dragon : Weapon
{
    public override void Fire(float dir)
    {
        switch(GetLevelIndex())
        {
            // Level 1 --------------------------------------------------------------------------------------------
            case(0):
            {
                // Shoot three fireballs with small random added to speeds
                const float angleoffset = 0.15f;
                const float speedstart  = 1.0f;
                const float speedend    = 1.1f;
                ShootProjectile(0, dir+angleoffset).MultiplySpeed(Random.Range(speedstart, speedend));    // Up
                ShootProjectile(0, dir).MultiplySpeed(Random.Range(speedstart, speedend));    // Straight
                ShootProjectile(0, dir-angleoffset).MultiplySpeed(Random.Range(speedstart, speedend));    // Down
            }
            break;

            // Level 2 --------------------------------------------------------------------------------------------
            case(1):
            {
                // Shoot three fireballs with small random added to speeds
                const float angleoffset = 0.18f;
                const float speedstart  = 1.1f;
                const float speedend    = 1.2f;
                ShootProjectile(0, dir+angleoffset).MultiplySpeed(Random.Range(speedstart, speedend));    // Up
                ShootProjectile(0, dir+angleoffset*0.3f).MultiplySpeed(Random.Range(speedstart, speedend));    // Up-Straight
                ShootProjectile(0, dir-angleoffset*0.3f).MultiplySpeed(Random.Range(speedstart, speedend));    // Down-Straight
                ShootProjectile(0, dir-angleoffset).MultiplySpeed(Random.Range(speedstart, speedend));    // Down
            }
            break;

            // Level 3 --------------------------------------------------------------------------------------------
            default:
            case(2):
            {
                // Shoot three fireballs with small random added to speeds
                const float angleoffset = 0.21f;
                const float speedstart  = 1.2f;
                const float speedend    = 1.4f;
                ShootProjectile(0, dir+angleoffset).MultiplySpeed(Random.Range(speedstart, speedend));    // Up
                ShootProjectile(0, dir+angleoffset*0.5f).MultiplySpeed(Random.Range(speedstart, speedend));    // Up-Straight
                ShootProjectile(0, dir).MultiplySpeed(Random.Range(speedstart, speedend));    // Straight
                ShootProjectile(0, dir-angleoffset*0.5f).MultiplySpeed(Random.Range(speedstart, speedend));    // Down-Straight
                ShootProjectile(0, dir-angleoffset).MultiplySpeed(Random.Range(speedstart, speedend));    // Down
            }
            break;
        }
        
    }
}
