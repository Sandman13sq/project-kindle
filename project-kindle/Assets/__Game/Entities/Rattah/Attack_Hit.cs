using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Hit : MasterObject
{
    //check to see if kindle was hit, if so, play bonk audio
    private void OnCollisionEnter2D(Collision2D other){
        Entity_Player player = other.collider.GetComponent<Entity_Player>();
        if(player != null){
            game.StopSound("RattahMiss");
            game.PlaySound("RattahAttack");
        }
    }
}
