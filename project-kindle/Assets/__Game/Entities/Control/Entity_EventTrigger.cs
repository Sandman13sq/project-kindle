using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_EventTrigger : Entity
{
    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if ( eventkey != "" && !game.EventIsRunning() )
        {
            var p = game.GetPlayer();
            CastHurtbox(castresults, LAYER_HURTBOX_BIT);

            foreach(var hit in castresults)
            {
                if (hit.collider == null) {continue;}
                Entity e = GetEntityFromCollider(hit.collider);
                if (e == null) {continue;}

                if (e.GetTag() == "player")
                {
                    game.RunEvent(eventkey);
                }
            }
        }
    }
}
