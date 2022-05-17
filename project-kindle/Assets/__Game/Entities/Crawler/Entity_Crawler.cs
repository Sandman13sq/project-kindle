using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Crawler : Entity
{
    private const float alertradius = 400.0f;
    private const float acc = 0.1f;
    private const float maxspeed = 2.2f;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        Physics2D.CircleCast(
            new Vector2(transform.position.x, transform.position.y),
            alertradius,
            new Vector2(0.0f, 0.0f),
            new ContactFilter2D() {layerMask=LAYER_ENTITY_BIT},
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
                    Vector3 ptransform = e.transform.position;

                    // If player is in sight
                    if (
                        !Physics2D.Linecast(
                            new Vector2(transform.position.x, transform.position.y),
                            new Vector2(ptransform.x, ptransform.y),
                            LAYER_WORLD_BIT
                            )
                        )
                    {
                        // To the left of player, move right
                        if (transform.position.x < ptransform.x)
                        {
                            xspeed = Mathf.Min(xspeed+acc*ts, maxspeed);
                        }
                        // To the right of player, move left
                        else
                        {
                            xspeed = Mathf.Max(xspeed-acc*ts, -maxspeed);
                        }

                        // Update sprite direction
                        spriterenderer.flipX = transform.position.x > ptransform.x;
                        break;
                    }
                    
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

    protected override void OnHealthChange(int diff, int weaponprojtype)
    {
        base.OnHealthChange(diff, weaponprojtype);

        if (diff < 0)
        {
            xspeed *= 0.3f;
        }
    }
}
