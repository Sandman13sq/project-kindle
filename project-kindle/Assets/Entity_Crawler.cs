using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Crawler : Entity
{
    private const float alertradius = 400.0f;
    private const float acc = 0.1f;
    private const float maxspeed = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] castresults = new RaycastHit2D[8];
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LAYER_ENTITY_BIT;
        
        Physics2D.CircleCast(
            new Vector2(transform.position.x, transform.position.y),
            alertradius,
            new Vector2(0.0f, 0.0f),
            filter,
            castresults,
            0.0f
        );
        
        // Iterate cast results
        foreach (RaycastHit2D e in castresults)
        {
            if (e.collider != null)
            {
                // Hit player
                if (e.collider.gameObject.tag == "player")
                {
                    float px = e.transform.position.x;

                    // To the left of player, move right
                    if (transform.position.x < px)
                    {
                        xspeed = Mathf.Min(xspeed+acc, maxspeed);
                    }
                    // To the right of player, move left
                    else
                    {
                        xspeed = Mathf.Max(xspeed-acc, -maxspeed);
                    }

                    // Update sprite direction
                    spriterenderer.flipX = transform.position.x > px;
                    break;
                }
            }
        }

        yspeed += -0.1f;
        UpdateDamageShake();
        UpdateMovement();
        
		if ( EvaluateCollision(CollisionFlag.CHANGESPEED | CollisionFlag.DOUBLEX).HasFlag(CollisionFlag.DOWN) )
		{
			yspeed = Mathf.Max(yspeed, 0.0f);
		}
    }
}
