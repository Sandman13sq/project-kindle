using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Dome_projectiles : Entity
{
    private bool hit = false;
    private float hitdelay = 10.0f;
    [SerializeField] private Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        yspeed = 10.0f;
        spriterenderer.sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();

        if (!hit)
        {
            if ( EvaluateCollision().HasFlag(CollisionFlag.ALL) )
            {
                hit = true;
            }
        }
        else if (hitdelay > 0.0f)
        {
            hitdelay = Mathf.Max(hitdelay-1.0f, 0.0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
