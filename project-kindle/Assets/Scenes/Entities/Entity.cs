using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bit flags for entity flag
public enum EntityFlag
{
    solid = 1 << 0,      // Calculate collision
    hostile = 2 << 0,    // Deals damage on contact. Mostly used for enemies and projectiles
}

public class Entity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Variables ======================================================

    // Internal
    public int entityindex; // Represents the position of entity when instanced
    public int entitytype;  // Marks class of entity. Value in EntityType enum
    
    public int entityflag;  // Bit field that represents entity options and attributes. See EntityFlag enum

    // Update
    public int state;   // Current state of entity. Used in Update()
    
    // Movement
    public float x;
    public float y;
    public float xspeed;
    public float yspeed;
    public float xacc;
    public float yacc;
    
    // Stats
    public int hitpointsmax;    // Number of hits an entity can take
    public int hitpoints;   // Remaining hitpoints
    public int attack;  // Damage dealt to player on contact
    public int energy;  // Energy dropped when defeated
    public int[] radius;  // Size of collision (x, y)
    public int radiusmode;  // 0 = circle

}
