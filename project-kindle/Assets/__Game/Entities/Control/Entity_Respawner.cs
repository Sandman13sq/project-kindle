using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Respawner : MasterObject
{
    private Entity entity = null;
    private Vector3 entityposition;
    private Vector3 initialposition;
    private float ignorerange = 2000.0f;
    private float triggerrange = 800.0f;
    private bool cantrigger = false;
    private bool entityactive = true;
    private bool onlyonce = false;

    // Start is called before the first frame update
    void Start()
    {
        initialposition = transform.position;
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
                if ( p.DistanceTo(initialposition) <= triggerrange )
                {
                    cantrigger = false;
                    entityactive = true;
                    entity.enabled = true;
                    entity.PositionSet(initialposition.x, initialposition.y);
                    entity.Restore();

                    if (onlyonce)
                    {
                        Destroy(this);
                        return;
                    }
                }
            }
            // Wait until player is out of range
            else
            {
                if ( p.DistanceTo(entityposition) > ignorerange )
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
    }

    public void ResetState(bool _cantrigger = false)
    {
        cantrigger = _cantrigger;
        entityactive = false;
        entity.DisableComponents();
        entity.enabled = false;
    }

    public void OnlyOnce()
    {
        onlyonce = true;
    }
}
