using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Effigy : Entity
{
    // Common ====================================================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        yspeed += -0.1f;
        UpdateDamageShake();
        UpdateMovement();
        EvaluateCollision(CollisionFlag.CHANGESPEED);
    }

    // Methods ====================================================================
}
