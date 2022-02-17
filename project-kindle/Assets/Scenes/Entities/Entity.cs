using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Variables ======================================================

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
    
    // Utility ============================================================

    

}
