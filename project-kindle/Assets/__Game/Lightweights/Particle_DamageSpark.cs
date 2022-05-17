using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_DamageSpark : MasterObject
{
    [SerializeField] SpriteRenderer spriterenderer;
    float life = 2.0f;
    float flipstep = 0.0f;
    
    // Update is called once per frame
    void Update()
    {
        // Wait to flip sprite
        if (flipstep > 0f) {flipstep -= game.GetTrueTimeStep();}
        else
        {
            spriterenderer.flipX = !spriterenderer.flipX;
            flipstep = 4.0f;
        }

        // Destroy when time step is restored
        if (life > 0.0f) {life -= 1.0f;}
        else if (game.GetActiveTimeStep() > 0f)
        {
            Destroy(gameObject);
        }
    }
}
