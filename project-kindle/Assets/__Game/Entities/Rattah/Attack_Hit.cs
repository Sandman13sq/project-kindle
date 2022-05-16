using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Hit : MasterObject
{
    bool bonkPlayed = false;
    int ticks = 0;

    private void Update() 
    {
        
        //Reset sound every 1 second
        if(ticks == 60)
        {
            bonkPlayed = false;
            ticks = 0;
        }
        ticks++;
    }
    //check to see if kindle was hit, if so, play bonk audio
    private void OnCollisionEnter2D(Collision2D other){
        Entity_Player player = other.collider.GetComponent<Entity_Player>();
        if(player != null && bonkPlayed == false){
            game.StopSound("RattahMiss");
            game.PlaySound("RattahAttack");
            bonkPlayed = true;
            ticks = 0;
        }
    }
}
