using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_DamageSpark : MasterObject
{
    [SerializeField] SpriteRenderer spriterenderer;
    float life = 2.0f;
    float flipstep = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (flipstep > 0f) {flipstep -= game.GetTrueTimeStep();}
        else
        {
            spriterenderer.flipX = !spriterenderer.flipX;
            flipstep = 4.0f;
        }

        if (life > 0.0f) {life -= 1.0f;}
        else if (game.GetActiveTimeStep() > 0f)
        {
            Destroy(gameObject);
        }
    }
}
