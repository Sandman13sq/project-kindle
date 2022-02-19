using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Energy0 : Entity
{
    private float gravity = -0.16f;
    private float terminal = -8.0f;
    [SerializeField] private float colorstep = 0.0f;

    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update color
        colorstep = Mathf.Repeat(colorstep + 0.02f, Mathf.PI);
        spriterenderer.color = Color.Lerp(color1, color2, Mathf.Sin(colorstep*colorstep));

        // Move
        yspeed += gravity;

        if ( (EasyCollision() & (1 <<3) ) != 0 )
        {
            yspeed = Mathf.Max( Mathf.Abs(yspeed) * 0.8f, 3.0f);
        };

        UpdateMovement();
    }
}
