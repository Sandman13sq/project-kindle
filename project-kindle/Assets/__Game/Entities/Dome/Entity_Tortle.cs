using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Tortle : Entity
{
    private RaycastHit2D playercast;
    private float strikeradius = 100.0f;
    private float statestep = 0.0f;
    private const float maxspeed = 1f;
    private const float speedslowmod = 0.95f;

    //animator for steam animation
    [SerializeField] GameObject steamPrefab;

    //boolean to see if the steam effect is already active
    private bool steamSpawned = false;

    //set the speed to 0 to stop the animation, set it back to 3.5 when we want it to play

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject strikeobj;

    enum State : int
    {
        Cruising = 1,   // Moving left/right, waiting for player
        Steam = 2,  // Expel Steam before attack
        Strike = 3, // Attack state. Shoot lightning bolt
        Grace = 4   // Cruising, but can't attack player
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        xspeed = spriterenderer.flipX? -1.0f: 1.0f; 
        state = (int)State.Cruising;
    }

    // Update is called once per frame
    protected override void Update()
    {
        yspeed -= ts*0.1f;

        UpdateMovement();
        UpdateDamageShake();

        CollisionFlag coll = EvaluateCollision(CollisionFlag.FINDEDGES);

        // Change direction if wall is hit or no floor is detected
        if (xspeed > 0.0f)
        {
            if (coll.HasFlag(CollisionFlag.RIGHT) || !coll.HasFlag(CollisionFlag.EDGERIGHT))
            {
                xspeed = -1.0f;
            }
        }
        else if (xspeed < 0.0f)
        {
            if (coll.HasFlag(CollisionFlag.LEFT) || !coll.HasFlag(CollisionFlag.EDGELEFT))
            {
                xspeed = 1.0f;
            }
        }
        if (coll.HasFlag(CollisionFlag.DOWN))
        {
            yspeed = 0.0f;
        }
        
        // Set sprite flip
        if (xspeed != 0f)
        {
            spriterenderer.flipX = xspeed < 0f;
        }
        
        spriterenderer.sprite = sprites[0];

        switch(state)
        {
            // Move left/right and check for player below ----------------------------------------
            case((int)State.Cruising):
            {
                spriterenderer.sprite = sprites[0];
                // Increase speed to maxspeed
                if (Mathf.Abs(xspeed) < maxspeed)
                {
                    xspeed = Mathf.Min(maxspeed, ts*Mathf.Abs(xspeed)+0.1f) * Mathf.Sign(xspeed);
                }
                
                if (statestep > 0) {statestep -= ts;}
                else
                {
                    // Circlecast for player
                    worldcollider.enabled = false;
                    hitboxcollider.enabled = false;
                    Physics2D.CircleCast(
                        new Vector2(transform.position.x, transform.position.y),
                        strikeradius,
                        new Vector2(0.0f, 0.0f),
                        new ContactFilter2D() {layerMask=LAYER_ENTITY_BIT},
                        castresults,
                        0.0f
                    );
                    worldcollider.enabled = true;
                    hitboxcollider.enabled = true;
                    
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
                                    state = (int)State.Steam;
                                    statestep = 40.0f;
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            }

            // before striking -------------------------------------------------------------
            case((int)State.Steam):
            {
                xspeed = ApproachTS(xspeed, 0.0f, 0.1f);
                GameObject steamObj;

                if(steamSpawned == false){
                    steamObj = Instantiate(steamPrefab);
                    steamObj.transform.position = transform.position;
                    steamSpawned = true;
                    Destroy(steamObj, 1.0f);
                }

                spriterenderer.sprite = sprites[1];

                if (statestep > 0) {statestep -= ts;}
                else
                {
                    steamSpawned = false;
                    state = (int)State.Strike;
                    statestep = 20.0f;

                    //Spawn 5 bullets, set each with a unique number
                    for(int i = 0; i < 5; i++){
                        game.PlaySound("DomeAttack"); 
                        GameObject obj = Instantiate(strikeobj);
                        obj.transform.position = transform.position + new Vector3(0, -16.0f, 0.0f);
                        obj.GetComponent<Entity_Dome_projectiles>().bulletNum = i;
                    }
                }
                break;
            }

            // Wait period after striking -----------------------------------------------------
            case((int)State.Strike):
            {
                xspeed = ApproachTS(xspeed, 0.0f, 0.1f);

                if (statestep > 0) {statestep -= ts;}
                else
                {
                    state = (int)State.Cruising;
                    statestep = 20.0f;
                }
                break;
            }
        }

        
    }
}
