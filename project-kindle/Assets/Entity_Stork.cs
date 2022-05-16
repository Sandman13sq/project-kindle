using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Stork : Entity
{
    [SerializeField] private SpriteRenderer bagrenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] bag_sprites;
    float image_index = 0f;
    float reloadtime = 200f;
    float reloadprogress = 0f;
    [SerializeField] private GameObject bag_prefab;
    [SerializeField] private Collider2D scan_collider;
    float xspeedsign;
    private float strikerange = 200.0f;
    private RaycastHit2D playercast;

    // ==========================================================

    // Start is called before the first frame update
    protected override void Start()
    {
        xspeedsign = spriterenderer.flipX? -1f: 1f;
        bagrenderer.enabled = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float ts = game.GetActiveTimeStep();

        // Sprite Update
        image_index = Mathf.Repeat(image_index + 0.13f*ts, sprites.Length);
        spriterenderer.sprite = sprites[(int)image_index];

        // Regenerate Bag
        if (reloadprogress > 0f)
        {
            reloadprogress -= ts;

            bagrenderer.enabled = false;
            if (reloadprogress <= 4f)   // Bag flash
            {
                bagrenderer.enabled = true;
                bagrenderer.sprite = bag_sprites[1];
            }
            if (reloadprogress == 0.0f)
            {
                bagrenderer.enabled = true;
                bagrenderer.sprite = bag_sprites[0];
            }
        }
        // Drop bag
        else
        {
            // Raycast for player
            worldcollider.enabled = false;
            hitboxcollider.enabled = false;
            playercast = Physics2D.Raycast(
                new Vector2(transform.position.x, transform.position.y), 
                new Vector2(0.0f, -1.0f), 
                strikerange,
                LAYER_ENTITY_BIT
                );
            worldcollider.enabled = true;
            hitboxcollider.enabled = true;
            
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
                        Instantiate(bag_prefab).transform.position = bagrenderer.transform.position;
                        bagrenderer.enabled = false;
                        reloadprogress = reloadtime;
                    }
                }
            }
        }

        UpdateDamageShake();

        // Swap direction when hitting the wall
        CollisionFlag coll = EvaluateCollision();

        if (
            (coll.HasFlag(CollisionFlag.RIGHT) && xspeedsign > 0f) || 
            (coll.HasFlag(CollisionFlag.LEFT) && xspeedsign < 0f)
            )
        {
            xspeedsign = -xspeedsign;
            spriterenderer.flipX = xspeedsign < 0f;
        }

        xspeed = ApproachTS(xspeed, 2f*xspeedsign);

        UpdateMovement();
    }

    protected override bool OnDefeat()
    {
        bagrenderer.enabled = false;
        return base.OnDefeat();
    }
}
