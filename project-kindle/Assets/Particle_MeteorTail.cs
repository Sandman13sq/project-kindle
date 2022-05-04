using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_MeteorTail : Particle_Simple
{
    // Update is called once per frame
    void Update()
    {
        spriterenderer.transform.localScale = new Vector2(1f-life/lifemax, 1f-life/lifemax);
        base.Update();
    }
}
