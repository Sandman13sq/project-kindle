using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Constants ======================================================

    [System.Flags]
    public enum CollisionFlag
    {
        RIGHT = 1 << 0, // Collision found on right side
        UP = 1 << 1,    // ^ top side
        LEFT = 1 << 2,  // ^ left side
        DOWN = 1 << 3,  // ^ bottom side

        CHANGESPEED = 1 << 0,   // Clamp speeds when a collision is found
        DOUBLEX = 1 << 1,   // Use two raycasts when checking left/right
        DOUBLEY = 1 << 2,   // Use two raycasts when checking up/down
    }

    // Variables ======================================================

    public SpriteRenderer spriterenderer;
    public Collider2D worldcollider;    // Used to interact with the world
    public Collider2D hitcollider;  // Used for hitbox collisions

    // Internal
    public int entityindex; // Represents the position of entity when instanced
    public int entitytype;  // Marks class of entity. Value in EntityType enum
    public string eventkey; // Key for event

    // Flags
    public bool solid;
    public bool ishostile;  // Damages player on contact
    public bool showdamage; // Shows damage numbers
    public bool isshootable;    // Takes damage from projectiles
    public bool eventondefeat;  // Calls event on defeat

    // Update
    public int state;   // Current state of entity. Used in Update()
    protected float xspeed;
    protected float yspeed;
    protected bool onground;
    
    // Stats
    public int hitpointsmax;    // Number of hits an entity can take
    public int hitpoints;   // Remaining hitpoints
    public int attack;  // Damage dealt to player on contact
    public int energy;  // Energy dropped when defeated

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Entity ================================================================
    
    // Called when pressing down on an entity
    public void Interact()
    {
        if (eventkey != "")
        {
            // Run Event
        }
    }

    // Adds speed to positions
    public void UpdateMovement()
    {
        transform.position = new Vector3(
            transform.position.x + xspeed,
            transform.position.y + yspeed,
            0.0f
        );
    }

    // Evaluates collision using "worldcollider" component
    public CollisionFlag EvaluateCollision(
        CollisionFlag settingsflag = CollisionFlag.CHANGESPEED,
        int layermask = 1<<3
        )
    {
        CollisionFlag collhit = 0;
        RaycastHit2D rayhit;
        float offset;
        int n;

        float x = transform.position.x;
        float y = transform.position.y;

        float offsetdown = Mathf.Abs(worldcollider.bounds.min.y-worldcollider.bounds.center.y);
        float offsetup = Mathf.Abs(worldcollider.bounds.max.y-worldcollider.bounds.center.y);
        float offsetright = Mathf.Abs(worldcollider.bounds.max.x-worldcollider.bounds.center.x);
        float offsetleft = Mathf.Abs(worldcollider.bounds.min.x-worldcollider.bounds.center.x);

        bool Cast(
            float xdir, 
            float ydir, 
            float offset, 
            out RaycastHit2D castresult,
            float xx,
            float yy
            )
        {
            worldcollider.enabled = false;
            castresult = Physics2D.Raycast(
                new Vector2(xx, yy), 
                new Vector2(xdir, ydir), 
                offset+1.0f,
                layermask
                );
            worldcollider.enabled = true;

            return castresult.collider != null;
        }

        //float[2] crossrange;

        // Down -----------------------------------------------------------------------
        if ( Cast(0.0f, -1.0f, offsetdown, out rayhit, x, y) )    // Collision found
        {
            y = rayhit.point.y+offsetdown;
            collhit |= CollisionFlag.DOWN;
            if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
            {
                yspeed = Mathf.Max(yspeed, 0.0f);
                onground = true;
            }
        }
        else if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
        {
            onground = false;
        }

        // Up -----------------------------------------------------------------------
        if ( Cast(0.0f, 1.0f, offsetdown, out rayhit, x, y) )    // Collision found
        {
            y = rayhit.point.y-offsetup;
            collhit |= CollisionFlag.UP;
            if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
            {
                yspeed = Mathf.Min(yspeed, 0.0f);
            }
        }

        // Right -----------------------------------------------------------------------
        if ( Cast(1.0f, 0.0f, offsetright, out rayhit, x, y) )    // Collision found
		{
			x = rayhit.point.x-offsetright;
            if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
            {
                xspeed = Mathf.Min(xspeed, 0.0f);
            }
            collhit |= CollisionFlag.RIGHT;
		}
        else if ( settingsflag.HasFlag(CollisionFlag.DOUBLEX) ) // Do bonus casts
        {
            if ( Cast(1.0f, 0.0f, offsetright, out rayhit, x, y-offsetdown*0.5f) )    // Collision found
            {
                x = rayhit.point.x-offsetright;
                collhit |= CollisionFlag.RIGHT;
                if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
                {
                    xspeed = Mathf.Min(xspeed, 0.0f);
                }
            }

            if ( Cast(1.0f, 0.0f, offsetright, out rayhit, x, y+offsetup*0.5f) )    // Collision found
            {
                x = rayhit.point.x-offsetright;
                collhit |= CollisionFlag.RIGHT;
                if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
                {
                    xspeed = Mathf.Min(xspeed, 0.0f);
                }
            }
        }

        // Left -----------------------------------------------------------------------
        if ( Cast(-1.0f, 0.0f, offsetleft, out rayhit, x, y) )    // Collision found
		{
			x = rayhit.point.x+offsetleft;
            collhit |= CollisionFlag.LEFT;
            if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
            {
                xspeed = Mathf.Max(xspeed, 0.0f);
            }
		}
        else if ( settingsflag.HasFlag(CollisionFlag.DOUBLEX) ) // Do bonus casts
        {
            if ( Cast(-1.0f, 0.0f, offsetright, out rayhit, x, y-offsetdown*0.5f) )    // Collision found
            {
                x = rayhit.point.x+offsetleft;
                collhit |= CollisionFlag.LEFT;
                if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
                {
                    xspeed = Mathf.Max(xspeed, 0.0f);
                }
            }

            if ( Cast(-1.0f, 0.0f, offsetright, out rayhit, x, y+offsetup*0.5f) )    // Collision found
            {
                x = rayhit.point.x+offsetleft;
                collhit |= CollisionFlag.LEFT;
                if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
                {
                    xspeed = Mathf.Max(xspeed, 0.0f);
                }
            }
        }

        // Update position
        transform.position = new Vector3(x, y, 0.0f);

        //  Return bitmask of collision hits
        return collhit;
    }

    // Utility ================================================================

    public void PositionSet(float _x, float _y)
    {
        transform.position = new Vector3(_x, _y, 0.0f);
    }
    public void PositionSetX(float x) {transform.position = new Vector3(x, transform.position.y, transform.position.z);}
    public void PositionSetY(float y) {transform.position = new Vector3(transform.position.x, y, transform.position.z);}
    public void SpeedSetX(float spd) {xspeed = spd;}
    public void SpeedSetY(float spd) {yspeed = spd;}
    public void SpeedAddX(float spd) {xspeed += spd;}
    public void SpeedAddY(float spd) {yspeed += spd;}

    

}
