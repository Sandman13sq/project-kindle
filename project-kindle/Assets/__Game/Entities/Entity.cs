using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DmrMath;

public class Entity : MasterObject
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
        ALL = RIGHT | UP | LEFT | DOWN,  // All sides
        EDGERIGHT = 1 << 4, // Set when FINDEDGES is active and a collision is found on the bottom-right
        EDGELEFT = 1 << 5,  // Set when FINDEDGES is active and a collision is found on the bottom-left

        CHANGESPEED = 1 << 0,   // Clamp speeds when a collision is found
        DOUBLEX = 1 << 1,   // Use two raycasts when checking left/right
        DOUBLEY = 1 << 2,   // Use two raycasts when checking up/down
        FINDEDGES = 1 << 3, // Check for bottom-right and bottom-left edges
    }

    protected const int LAYER_WORLD_INDEX = 3;
    protected const int LAYER_WORLD_BIT = 1 << LAYER_WORLD_INDEX;
    protected const int LAYER_ENTITY_INDEX = 6; 
    protected const int LAYER_ENTITY_BIT = 1 << LAYER_ENTITY_INDEX;
    protected const int LAYER_HITBOX_INDEX = 7; 
    protected const int LAYER_HITBOX_BIT = 1 << LAYER_HITBOX_INDEX;
    protected const int LAYER_HURTBOX_INDEX = 8; 
    protected const int LAYER_HURTBOX_BIT = 1 << LAYER_HURTBOX_INDEX;

    // Variables ======================================================

    [SerializeField] protected string eventkey = ""; // Key for event
    [SerializeField] protected string entitytag = ""; // Tag used to reference by event/entity
    
    public SpriteRenderer spriterenderer;
    public Collider2D hitboxcollider;  // Used for damaging player on contact
    public Collider2D hurtboxcollider;  // Used for damaging this entity on contact
    public Collider2D worldcollider;    // Used to interact with the world
    public Rigidbody2D rbody;  // Necessary for collision detection

    // Flags
    //public bool solid;
    //public bool ishostile;  // Damages player on contact
    public bool showdamage; // Shows damage numbers
    public bool isshootable;    // Takes damage from projectiles
    public bool eventondefeat;  // Calls event on defeat
    public bool dorespawn;  // Respawns when 1.5 screens away
    public bool enableinrange; // Enabled when 1.5 screens away

    // Update
    public int state;   // Current state of entity. Used in Update()
    protected float xspeed;
    protected float yspeed;
    protected bool onground;
    protected float damageshake = 0.0f;
    protected float damageshaketime = 10.0f;
    protected float rightsign = 1.0f; // 1 when facing right, -1 when facing left

    protected RaycastHit2D[] castresults = new RaycastHit2D[8]; // Used for cast results. Initialized once for optimization
    
    // Stats
    public int health;   // Remaining hitpoints
    public int healthmax;    // Number of hits an entity can take
    public int attack;  // Damage dealt to player on contact
    public int energy; // Energy dropped when defeated (CURRENTLY UNUSED)
    [SerializeField] private GameObject[] energydrops;  // Energy objects dropped when defeated
    [SerializeField] private GameObject heartdrop;
    [SerializeField] private GameObject[] destroygraphic;

    [SerializeField] private GameObject shownumberprefab;
    private GameObject shownumberobj = null;
    private Entity_Respawner respawner = null;
    private int startingstate;

    protected float ts {get {return game.TimeStep;}}

    // =============== Audio stuff ===============
    private bool soundPlayed = false;
    //============================================

    // Common ================================================================

    // Called on creation
    void Awake()
    {
        startingstate = state;

        // Add parent entity component to colliders
        if (hitboxcollider != null && hitboxcollider.gameObject != this)
        {(hitboxcollider.gameObject.AddComponent(typeof(ParentEntity)) as ParentEntity).SetEntity(this);}

        if (hurtboxcollider != null && hurtboxcollider.gameObject != this)
        {(hurtboxcollider.gameObject.AddComponent(typeof(ParentEntity)) as ParentEntity).SetEntity(this);}

        if (worldcollider != null && worldcollider.gameObject != this)
        {(worldcollider.gameObject.AddComponent(typeof(ParentEntity)) as ParentEntity).SetEntity(this);}

        // Create respawn component
        if (dorespawn || enableinrange)
        {
            // Create respawner component
            respawner = gameObject.AddComponent(typeof(Entity_Respawner)) as Entity_Respawner;
            respawner.SetEntity(this);

            // Trigger respawner only once
            if (!dorespawn)
            {
                respawner.OnlyOnce();
            }

            // Disable until Kindle is in range
            if (enableinrange)
            {
                respawner.ResetState(true);
                return;
            }
        }
    }

    // Start is called before the first frame update
    protected virtual void Start() {}
    
    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateDamageShake();    // Update shake when damaged
        UpdateMovement();       // Add x and y speeds to position
        EvaluateCollision();    // Evaluate collision with world geometry
    }
    
    // Entity ================================================================

    public void ResetValues()
    {
        health = healthmax;
        if (spriterenderer != null) {spriterenderer.enabled = true;}
        if (hitboxcollider != null) {hitboxcollider.enabled = true;}
        if (hurtboxcollider != null) {hurtboxcollider.enabled = true;}
        if (worldcollider != null) {worldcollider.enabled = true;}

        xspeed = 0.0f;
        yspeed = 0.0f;

        //state = startingstate;
    }

    // Disables entity components (Renderer and collision boxes)
    public void DisableComponents()
    {
        if (spriterenderer != null) {spriterenderer.enabled = false;}
        if (hitboxcollider != null) {hitboxcollider.enabled = false;}
        if (hurtboxcollider != null) {hurtboxcollider.enabled = false;}
        if (worldcollider != null) {worldcollider.enabled = false;}
    }

    // Called when re activated by the entity respawner
    public void Restore()
    {
        ResetValues();
        Start();
    }
    
    // Called when pressing down on an entity
    public virtual void Interact()
    {
        //if (eventkey != "")
        {
            // Run Event
        }
    }

    // Called in Defeat() call before destruction
    protected virtual bool OnDefeat()
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

        return true;
    }

    // Called when resulting health from ChangeHealth is zero
    public void Defeat()
    {
        bool _destroy = OnDefeat();

        // Run death event
        if (eventondefeat && game.EventExists(eventkey))
        {
            game.RunEvent(eventkey);
        }

        // Run 
        if (dorespawn)
        {
            respawner.ResetState();
        }
        else if (_destroy)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnHealthChange(int change)
    {
        if (change < 0)
        {
            damageshake = damageshaketime;
        }
    }

    // Drops energy amount. Called in Defeat()
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

    // Drops heart. Called in Defeat()
    protected void DropHeart()
    {
        if (heartdrop)
        {
            Entity e = Instantiate(heartdrop).GetComponent<Entity>();
            e.transform.position = transform.position;
        }
    }

    // Shows destroy graphics. Called in Defeat()
    protected void ShowDestroyGraphic()
    {
        foreach (GameObject prefab in destroygraphic)
        {
            Instantiate(prefab).transform.position = transform.position;
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

            // Show damage
            if (showdamage)
            {
                if (shownumberobj == null)
                {
                    shownumberobj = Instantiate(shownumberprefab);
                    shownumberobj.GetComponent<ShowNumber>().SetTargetObject(gameObject);
                    shownumberobj.GetComponent<ShowNumber>().ZeroValue();
                }

                shownumberobj.GetComponent<ShowNumber>().AddValue(ShowNumber.NumberType.HEALTH, value);
            }

            OnHealthChange(health-prehealth);

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

            OnHealthChange(health-prehealth);
        }

        return health-prehealth; // Return change in health
    }

    // ChangeHealth() shortcut
    public virtual int DoDamage(int value)
    {
        return ChangeHealth(-value);
    }

    // Adds speed to positions
    public void UpdateMovement()
    {
        transform.position = new Vector3(
            transform.position.x + xspeed * ts,
            transform.position.y + yspeed * ts,
            transform.position.z
        );
    }

    // Updates shaking when taking damage
    protected void UpdateDamageShake()
    {
        if (damageshake > 0)
        {
            damageshake = ApproachTS(damageshake, 0.0f);

            // Exit if there's no sprite renderer set
            if (spriterenderer == null) {return;}

            // Set x offset
            if (damageshake > 0)
            {
                float xshift = (BoolStep(damageshake, 4.0f) == 1.0f? -3: 3); // Shift left/right every 4 frames
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
        if ( Cast(0.0f, 1.0f, offsetup, out rayhit, x, y) )    // Collision found
        {
            y = rayhit.point.y-offsetup-1.0f;
            collhit |= CollisionFlag.UP;
            if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
            {
                yspeed = Mathf.Min(yspeed, 0.0f);
            }
        }

        // Right -----------------------------------------------------------------------
        if ( Cast(1.0f, 0.0f, offsetright, out rayhit, x, y) )    // Collision found
		{
			x = rayhit.point.x-offsetright-1.0f;
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
                x = rayhit.point.x-offsetright-1.0f;
                collhit |= CollisionFlag.RIGHT;
                if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
                {
                    xspeed = Mathf.Min(xspeed, 0.0f);
                }
            }

            if ( Cast(1.0f, 0.0f, offsetright, out rayhit, x, y+offsetup*0.5f) )    // Collision found
            {
                x = rayhit.point.x-offsetright-1.0f;
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
			x = rayhit.point.x+offsetleft+1.0f;
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
                x = rayhit.point.x+offsetleft+1.0f;
                collhit |= CollisionFlag.LEFT;
                if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
                {
                    xspeed = Mathf.Max(xspeed, 0.0f);
                }
            }

            if ( Cast(-1.0f, 0.0f, offsetright, out rayhit, x, y+offsetup*0.5f) )    // Collision found
            {
                x = rayhit.point.x+offsetleft+1.0f;
                collhit |= CollisionFlag.LEFT;
                if ( settingsflag.HasFlag(CollisionFlag.CHANGESPEED) )
                {
                    xspeed = Mathf.Max(xspeed, 0.0f);
                }
            }
        }

        // Edges ---------------------------------------------------------------------
        if ( settingsflag.HasFlag(CollisionFlag.FINDEDGES) )
        {
            if ( Cast(0.0f, -1.0f, offsetdown, out rayhit, x+offsetright, y-1.0f) ) // Right edge
            {
                collhit |= CollisionFlag.EDGERIGHT;
            }

            if ( Cast(0.0f, -1.0f, offsetdown, out rayhit, x-offsetleft, y-1.0f) ) // Left edge
            {
                collhit |= CollisionFlag.EDGELEFT;
            }
        }

        // Update position
        transform.position = new Vector3(x, y, transform.position.z);

        //  Return bitmask of collision hits
        return collhit;
    }

    // Utility ================================================================

    public void ResetHealth()
    {
        health = healthmax;
    }

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

    public float DistanceTo(Entity e)
    {
        float xx = e.transform.position.x - transform.position.x;
        float yy = e.transform.position.y - transform.position.y;
        return Mathf.Sqrt(xx*xx+yy*yy);
    }
    public float DistanceTo(Vector3 v)
    {
        float xx = v.x - transform.position.x;
        float yy = v.y - transform.position.y;
        return Mathf.Sqrt(xx*xx+yy*yy);
    }
    public float DistanceTo(Vector2 v)
    {
        float xx = v.x - transform.position.x;
        float yy = v.y - transform.position.y;
        return Mathf.Sqrt(xx*xx+yy*yy);
    }

    public float SignToX(Entity e) {return Mathf.Sign(e.transform.position.x-transform.position.x);}

    public int GetAttack() {return attack;}

    // Returns root entity from collider if exists, null otherwise.
    static protected Entity GetEntityFromCollider(Collider2D c)
    {
        if (c != null)
        {
            if (c.gameObject.TryGetComponent(out ParentEntity p))
            {
                return p.GetEntity();
            }
            
            if (c.gameObject.transform.parent != null && c.gameObject.transform.parent.TryGetComponent(out Entity e))
            {
                return e;
            }
        }
        
        return null;
    }

    // Casts hurtbox against hitboxes and populates results in given variable. Returns number of results
    protected int CastHurtbox(RaycastHit2D[] hitresults, int mask = LAYER_HITBOX_BIT)
    {
        if (hurtboxcollider == null) {return 0;}

        hurtboxcollider.Cast(
            new Vector2(0.0f, 0.0f),
            new ContactFilter2D() {layerMask=mask, useLayerMask=true},
            hitresults,
            0.0f,
            true
        );

        return hitresults.Length;
    }

    public void ClearNumberObject()
    {
        shownumberobj = null;
    }

    public string GetTag()
    {
        return entitytag;
    }

    // Returns first entity with given tag
    static public Entity FindEntity(string _tag)
    {
        foreach (Entity e in FindObjectsOfType<Entity>())
        {
            if (e.GetTag() == _tag)
            {
                return e;
            }
        }

        return null;
    }

    // Returns list of entities with given tag
    static public List<Entity> FindEntities(string _tag)
    {
        List<Entity> outentities = new List<Entity>();
        foreach (Entity e in FindObjectsOfType<Entity>())
        {
            if (e.GetTag() == _tag)
            {
                outentities.Add(e);
            }
        }

        return outentities;
    }
}
