using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Respawner : MasterObject
{
    private Entity entity = null;
    private Vector3 entityposition;
    private float ignorerange = 1600.0f;
    private float triggerrange = 1000.0f;
    private bool cantrigger = false;
    private bool entityactive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (entity != null)
        {
            var p = game.GetPlayer();

            // Reactivate entity
            if (cantrigger && !entityactive)
            {
                if ( p.DistanceTo(transform.position) <= triggerrange )
                {
                    cantrigger = false;
                    entityactive = true;
                    entity.enabled = true;
                    entity.PositionSet(entityposition.x, entityposition.y);
                    entity.Restore();
                    Debug.Log("Enabling entity");
                }
            }
            // Wait until player is out of range
            else
            {
                if ( p.DistanceTo(transform.position) > ignorerange )
                {
                    cantrigger = true;
                }
            }
        }    
    }

    public void SetEntity(Entity e)
    {
        entity = e;
        entityactive = true;
        entityposition = entity.transform.position;
        Debug.Log(entityposition);
    }

    public void ResetState()
    {
        cantrigger = false;
        entityactive = false;
        entity.DisableComponents();
        entity.enabled = false;
    }
}
