using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Energy0 : Entity
{
    private float gravity = -0.16f;
    private float terminalvelocity = -8.0f;
    private float colorstep = 0.0f;

    private Color color1 = new Color(0.5f, 1.0f, 1.0f);
    private Color color2 = new Color(1.0f, 1.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        colorstep = Random.Range(0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Update color
        colorstep = Mathf.Repeat(colorstep + 0.02f, Mathf.PI);
        spriterenderer.color = Color.Lerp(color1, color2, Mathf.Sin(colorstep*colorstep));

        // Move
        if (yspeed > terminalvelocity)
        {
            yspeed = Mathf.Max(yspeed+gravity, terminalvelocity);
        }

        // If ground collision...
        if ( (EvaluateCollision() & CollisionFlag.DOWN ) != 0 )
        {
            // Dampen yspeed on each bounce
            yspeed = Mathf.Max( Mathf.Abs(yspeed) * 0.8f, 3.0f);
        };

        UpdateMovement();
    }
}
