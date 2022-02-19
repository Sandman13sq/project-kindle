using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
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
    public float xspeed;
    public float yspeed;
    public bool onground;
    
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
    public int EasyCollision(int flag = 0)
    {
        int collhit = 0;

        float ydiff = Mathf.Abs(worldcollider.bounds.min.y-worldcollider.bounds.center.y);

        // Down
        worldcollider.enabled = false;
        RaycastHit2D groundcollision = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y), 
            new Vector2(0.0f, -1.0f), 
            ydiff+1.0f
            );
        worldcollider.enabled = true;

        if (groundcollision.collider != null)
		{
			transform.position = new Vector3(transform.position.x, groundcollision.point.y+ydiff, 0.0f);
			onground = true;

            collhit |= 1 << 3;
		}
		else
		{
			onground = false;
		}

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
