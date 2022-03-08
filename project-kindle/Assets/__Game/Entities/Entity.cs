using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Constants ======================================================

    [System.Flags]
    public enum CollisionFlag
    {
        NONE = 0,
        
        RIGHT = 1 << 0, // Collision found on right side
        UP = 1 << 1,    // ^ top side
        LEFT = 1 << 2,  // ^ left side
        DOWN = 1 << 3,  // ^ bottom side
        ALL = 0xF,  // All sides

        CHANGESPEED = 1 << 0,   // Clamp speeds when a collision is found
        DOUBLEX = 1 << 1,   // Use two raycasts when checking left/right
        DOUBLEY = 1 << 2,   // Use two raycasts when checking up/down
    }

    protected const int LAYER_WORLD = 3;
    protected const int LAYER_WORLD_BIT = 1 << LAYER_WORLD;
    protected const int LAYER_ENTITY = 6; 
    protected const int LAYER_ENTITY_BIT = 1 << LAYER_ENTITY;
    protected const int LAYER_HITBOX = 7; 
    protected const int LAYER_HITBOX_BIT = 1 << LAYER_HITBOX;

    // Variables ======================================================

    public SpriteRenderer spriterenderer;
    public Collider2D worldcollider;    // Used to interact with the world
    public Collider2D hitcollider;  // Used for hitbox collisions
    public Rigidbody2D rigidbody;  // Necessary for collision detection

    // Internal
    //public int entityindex; // Represents the position of entity when instanced
    //public int entitytype;  // Marks class of entity. Value in EntityType enum
    //public string eventkey; // Key for event

    // Flags
    //public bool solid;
    //public bool ishostile;  // Damages player on contact
    public bool showdamage; // Shows damage numbers
    public bool isshootable;    // Takes damage from projectiles
    public bool eventondefeat;  // Calls event on defeat

    // Update
    public int state;   // Current state of entity. Used in Update()
    protected float xspeed;
    protected float yspeed;
    protected bool onground;
    protected float damageshake = 0.0f;
    protected float damageshaketime = 10.0f;
    protected float rightsign = 1.0f; // 1 when facing right, -1 when facing left
    
    // Stats
    public int health;   // Remaining hitpoints
    public int healthmax;    // Number of hits an entity can take
    public int attack;  // Damage dealt to player on contact
    public int energy; // Energy dropped when defeated (CURRENTLY UNUSED)
    [SerializeField] private GameObject[] energydrops;  // Energy objects dropped when defeated
    [SerializeField] private GameObject heartdrop;
    [SerializeField] private GameObject[] destroygraphic;
    
    // Common ================================================================

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateDamageShake();    // Update shake when damaged
        UpdateMovement();       // Add x and y speeds to position
        EvaluateCollision();    // Evaluate collision with world geometry
    }
    
    // Entity ================================================================
    
    // Called when pressing down on an entity
    public virtual void Interact()
    {
        //if (eventkey != "")
        {
            // Run Event
        }
    }

    // Drops energy amount
    protected void DropEnergy()
    {
        Entity e;
        foreach (GameObject obj in energydrops)
        {
            e = Instantiate(obj).GetComponent<Entity>();
            e.transform.position = transform.position;
            e.SpeedSetDeg(
                Random.Range(2.0f, 5.0f),
                Random.Range(80.0f, 100.0f)
                );
        }
    }

    protected void DropHeart()
    {
        if (heartdrop)
        {
            Entity e = Instantiate(heartdrop).GetComponent<Entity>();
            e.transform.position = transform.position;
        }
    }

    protected void ShowDestroyGraphic()
    {
        foreach (GameObject prefab in destroygraphic)
        {
            Instantiate(prefab).transform.position = transform.position;
        }
    }

    // Called in Defeat() call before destruction
    public virtual void OnDefeat()
    {
        if (heartdrop && Random.Range(0.0f, 1.0f) < 0.3f)
        {
            DropHeart();
        }
        else
        {
            DropEnergy();
        }

        ShowDestroyGraphic();
    }

    // Called when resulting health from ChangeHealth is zero
    public virtual void Defeat()
    {
        OnDefeat();
        Destroy(gameObject);
    }

    protected virtual void OnHealthChange(int change)
    {
        if (change < 0)
        {
            damageshake = damageshaketime;
        }
    }

    // Changes health value by amount. Returns change in health
    public virtual int ChangeHealth(int value)
    {
        int prehealth = health;

        // Losing health
        if (value < 0)
        {
            health += value;

            if (health < 0) 
            {
                health = 0;
                Defeat();
            }
        }
        // Gaining health
        else if (value > 0)
        {
            health += value;
            if (health > healthmax) {health = healthmax;}
        }

        OnHealthChange(health-prehealth);

        return health-prehealth; // Return change in health
    }

    public virtual int DoDamage(int value)
    {
        return ChangeHealth(-value);
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

    // Updates shaking when taking damage
    protected void UpdateDamageShake()
    {
        if (damageshake > 0)
        {
            damageshake = Mathf.Max(0.0f, damageshake - 1.0f);

            // Set x offset
            if (damageshake > 0)
            {
                float xshift = ((Mathf.Repeat(damageshake, 4.0f) < 2.0f)? -3: 3); // Shift left/right every 4 frames
                spriterenderer.transform.localPosition = new Vector3(
                    xshift, 
                    spriterenderer.transform.localPosition.y, 
                    spriterenderer.transform.localPosition.z
                    );
            }
            // Reset offset
            else
            {
                spriterenderer.transform.localPosition = new Vector3(
                    spriterenderer.transform.localPosition.x, 
                    spriterenderer.transform.localPosition.y, 
                    spriterenderer.transform.localPosition.z
                    );
            }
        }
    }

    // Evaluates collision using "worldcollider" component
    public CollisionFlag EvaluateCollision(
        CollisionFlag settingsflag = CollisionFlag.CHANGESPEED,
        int layermask = LAYER_WORLD_BIT
        )
    {
        // Yeet out if there's no world collider
        if (worldcollider == null) {return CollisionFlag.NONE;}

        CollisionFlag collhit = 0;
        RaycastHit2D rayhit;

        float x = transform.position.x;
        float y = transform.position.y;

        Bounds cbounds = worldcollider.bounds;
        Vector2 coffset = worldcollider.offset;

        float offsetdown = Mathf.Abs(cbounds.min.y-cbounds.center.y+coffset.y);
        float offsetup = Mathf.Abs(cbounds.max.y-cbounds.center.y+coffset.y);
        float offsetright = Mathf.Abs(cbounds.max.x-cbounds.center.x+coffset.x);
        float offsetleft = Mathf.Abs(cbounds.min.x-cbounds.center.x+coffset.x);

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

    public void SpeedSetRad(float _speed, float _direction)
    {
        xspeed = Mathf.Cos(_direction) * _speed;
        yspeed = Mathf.Sin(_direction) * _speed;
    }
    public void SpeedAddRad(float _speed, float _direction)
    {
        xspeed = Mathf.Cos(_direction) * _speed;
        yspeed = Mathf.Sin(_direction) * _speed;
    }
    public void SpeedSetDeg(float _speed, float _direction) {SpeedSetRad(_speed, _direction * Mathf.Deg2Rad);}
    public void SpeedAddDeg(float _speed, float _direction) {SpeedAddRad(_speed, _direction * Mathf.Deg2Rad);}

    public int GetAttack() {return attack;}
}