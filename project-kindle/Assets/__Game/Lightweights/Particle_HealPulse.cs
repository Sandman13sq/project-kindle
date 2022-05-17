using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_HealPulse : Particle_Simple
{
    // Update is called once per frame
    protected override void Update()
    {
        transform.position += new Vector3(0f, 2f, 0f);

        base.Update();
    }
}
