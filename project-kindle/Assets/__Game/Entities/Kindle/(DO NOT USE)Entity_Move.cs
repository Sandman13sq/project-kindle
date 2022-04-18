using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveAttributes
{
    [SerializeField] public float acc = 20.0f;   // Acceleration
    [SerializeField] public float dec = 40.0f;   // Deceleration
    [SerializeField] public float airacc = 16.0f;
    [SerializeField] public float airdec = 20.0f;
    [SerializeField] public float maxspeed = 256.0f;    // Max xspeed
    [SerializeField] public float terminal = -400.0f;   // Max yspeed falling down
    [SerializeField] public float gravity = -16.0f; // Normal Gravity
    [SerializeField] public float gravityjump = -7.0f; // Used when rising and jump is held
    [SerializeField] public float jumpstrength = 275.0f;
}

public class Entity_Move : MonoBehaviour
{
    // Variables =========================================
    public bool inair;
    public bool onground;
    public bool jumpheld;
    public float jumpbuffer;
    private float jumpbuffertime = 7.0f; // Max number of frames ahead of time where a jump press will still be read

    private Rigidbody2D rb;
    private Collider2D collider2d;
    private SpriteRenderer spriterdr;

    [SerializeField] private MoveAttributes mvattr;

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 60; // Temporary. Will remove later

        rb = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        spriterdr = GetComponent<SpriteRenderer>();
        mvattr = new MoveAttributes();

        rb.drag = 0.0f;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Grab input values
        float xlev = Input.GetAxisRaw("Horizontal");
        float ylev = Input.GetAxisRaw("Vertical");

        // Flip sprite if moving left
        if (xlev != 0.0)
        {
            spriterdr.flipX = xlev < 0.0f;
        }

        // Jump buffer
        if (jumpbuffer >= 0.0f)
        {
            jumpbuffer -= 1.0f;
        }

        if (Input.GetKeyDown("z"))
        {
            jumpbuffer = jumpbuffertime;
        }

        // Ground Movement --------------------------------------------------------------------
        if (onground)
        {
            /*
                When player is moving in the same direction as the input, use acceleration and approach movespeed.
                When player is moving in the opposing direction as the input, use deceleration and approach movespeed.
                When no input is held, use deceleration and approach 0.
            */
            if ( xlev > 0.0f && rb.velocity.x < mvattr.maxspeed ) // Moving right
            {
                rb.AddForce(new Vector2(xlev * ( (Mathf.Sign(xlev)==Mathf.Sign(rb.velocity.x)? mvattr.acc: mvattr.dec) ), 0.0f), ForceMode2D.Impulse);
                
                if (rb.velocity.x > mvattr.maxspeed) // Clamp speed
                {
                    rb.velocity = new Vector2(mvattr.maxspeed, rb.velocity.y);
                }
            }
            else if ( xlev < 0.0f && rb.velocity.x > -mvattr.maxspeed ) // Moving left
            {
                rb.AddForce(new Vector2(xlev * ( (Mathf.Sign(xlev)==Mathf.Sign(rb.velocity.x)? mvattr.acc: mvattr.dec) ), 0.0f), ForceMode2D.Impulse);
                
                if (rb.velocity.x < -mvattr.maxspeed) // Clamp speed
                {
                    rb.velocity = new Vector2(-mvattr.maxspeed, rb.velocity.y);
                }
            }
            else    // No input held
            {
                if (Mathf.Abs(rb.velocity.x) <= mvattr.dec) // Clamp to 0 when speed is small enough
                {
                    rb.velocity *= new Vector2(0.0f, 1.0f);
                }
                else    // Approach 0 with deceleration
                {
                    rb.AddForce(new Vector2(-Mathf.Sign(rb.velocity.x) * mvattr.dec, 0.0f), ForceMode2D.Impulse);
                }
            }

            // Jump
            if ( jumpbuffer > 0.0f )
            {
                rb.velocity += new Vector2(0.0f, mvattr.jumpstrength);
                jumpheld = true;
                jumpbuffer = 0.0f; // Clear buffer
            }
        }
        // Air movement -----------------------------------------------------------------------------
        else
        {
            jumpheld = jumpheld && Input.GetKey("z") && rb.velocity.y > 0.0f;

            // Gravity
            if (rb.velocity.y >= mvattr.terminal)
            {
                rb.AddForce(new Vector2(0.0f, jumpheld? mvattr.gravityjump: mvattr.gravity), ForceMode2D.Impulse);
                
                if (rb.velocity.y < mvattr.terminal)
                {
                    rb.velocity = new Vector2(rb.velocity.x, mvattr.terminal);
                }
            }

            // Movement
            if ( xlev > 0.0f && rb.velocity.x < mvattr.maxspeed ) // Moving right
            {
                rb.AddForce(new Vector2(xlev * mvattr.airacc, 0.0f), ForceMode2D.Impulse);
                
                if (rb.velocity.x > mvattr.maxspeed) // Clamp speed
                {
                    rb.velocity = new Vector2(mvattr.maxspeed, rb.velocity.y);
                }
            }
            else if ( xlev < 0.0f && rb.velocity.x > -mvattr.maxspeed ) // Moving left
            {
                rb.AddForce(new Vector2(xlev * mvattr.airacc, 0.0f), ForceMode2D.Impulse);
                
                if (rb.velocity.x < -mvattr.maxspeed) // Clamp speed
                {
                    rb.velocity = new Vector2(-mvattr.maxspeed, rb.velocity.y);
                }
            }
        }

        // Check ground collision
        bool lastonground = onground;

        float ydiff = collider2d.bounds.extents.y;
        
        collider2d.enabled = false;   // Disable this collider so it doesn't hit itself
        RaycastHit2D groundcollision = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y-ydiff), 
            new Vector2(0.0f, -1.0f), 
            10.0f
            );
        collider2d.enabled = true;    // Re-enable collider
        onground = groundcollision.collider != null;
        inair = !onground;

        if ( (onground != lastonground) && onground)
        {
            // Snap to top of collision
            transform.position = new Vector3(transform.position.x, groundcollision.point.y+ydiff, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, 0.0f));
        }
    }


}
