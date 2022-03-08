using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_EyeDrop : Entity
{
    private RaycastHit2D playercast;
    private float strikerange = 200.0f;
    private float statestep = 0.0f;
    private const float maxspeed = 1.5f;
    private const float speedslowmod = 0.95f;
    private float hoverstep = 0.0f;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject strikeobj;

    enum State : int
    {
        Floating = 1,   // Moving left/right, waiting for player
        Flash = 2,  // Flash before attack
        Strike = 3, // Attack state. Shoot lightning bolt
        Grace = 4   // Floating, but can't attack player
    }

    // Start is called before the first frame update
    void Start()
    {
        xspeed = spriterenderer.flipX? -1.0f: 1.0f; 
        state = (int)State.Floating;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateDamageShake();

        CollisionFlag coll = EvaluateCollision(0);

        // Change direction if wall is hit
        if (
            (coll.HasFlag(CollisionFlag.RIGHT) && !spriterenderer.flipX) ||
            (coll.HasFlag(CollisionFlag.LEFT) && spriterenderer.flipX)
            )
        {
            spriterenderer.flipX = !spriterenderer.flipX;
            xspeed = Mathf.Max(0.01f, Mathf.Abs(xspeed)) * (spriterenderer.flipX? -1.0f: 1.0f);
            UpdateMovement();
        }

        spriterenderer.sprite = sprites[0];

        // States
        switch(state)
        {
            // Move left/right and check for player below ----------------------------------------
            case((int)State.Floating):
            {
                // Increase speed to maxspeed
                if (Mathf.Abs(xspeed) < maxspeed)
                {
                    xspeed = Mathf.Min(maxspeed, Mathf.Abs(xspeed)+0.1f) * Mathf.Sign(xspeed);
                }

                hoverstep = Mathf.Repeat(hoverstep+0.05f, 2.0f*Mathf.PI);
                yspeed = Mathf.Sin(hoverstep) * 0.4f;
                
                if (statestep > 0) {statestep -= 1.0f;}
                else
                {
                    // Raycast for player
                    worldcollider.enabled = false;
                    hitcollider.enabled = false;
                    playercast = Physics2D.Raycast(
                        new Vector2(transform.position.x, transform.position.y), 
                        new Vector2(0.0f, -1.0f), 
                        strikerange,
                        LAYER_ENTITY_BIT
                        );
                    worldcollider.enabled = true;
                    hitcollider.enabled = true;
                    
                    // Check collider
                    if (playercast.collider != null)
                    {
                        if (playercast.collider.gameObject.tag == "player")
                        {
                            Vector3 ptransform = playercast.collider.transform.position;
                            
                            // If player is in sight
                            if (
                                !Physics2D.Linecast(
                                    new Vector2(transform.position.x, transform.position.y),
                                    new Vector2(ptransform.x, ptransform.y),
                                    LAYER_WORLD_BIT
                                    )
                                )
                            {
                                state = (int)State.Flash;
                                statestep = 40.0f;
                            }
                        }
                    }
                }
                break;
            }

            // Flash before striking -------------------------------------------------------------
            case((int)State.Flash):
            {
                xspeed *= speedslowmod;

                spriterenderer.sprite = sprites[(Mathf.Repeat(statestep, 4.0f) < 2.0f)? 1: 0];

                if (statestep > 0) {statestep -= 1.0f;}
                else
                {
                    state = (int)State.Strike;
                    statestep = 20.0f;

                    GameObject obj = Instantiate(strikeobj);
                    obj.transform.position = transform.position + new Vector3(0.0f, -16.0f, 0.0f);
                }
                break;
            }

            // Wait period after striking -----------------------------------------------------
            case((int)State.Strike):
            {
                xspeed *= speedslowmod;

                if (statestep > 0) {statestep -= 1.0f;}
                else
                {
                    state = (int)State.Floating;
                    statestep = 20.0f;
                }
                break;
            }
        }

        
    }
}