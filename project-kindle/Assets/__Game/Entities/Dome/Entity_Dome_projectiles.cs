using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Dome_projectiles : Entity
{
    private bool hit = false;
    private float hitdelay = 10.0f;
    [SerializeField] private Sprite[] sprites;

    public int bulletNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriterenderer.sprite = sprites[0];
        if (bulletNum == 0){ xspeed = -10; }
        else if (bulletNum == 1){ xspeed = -10; yspeed = 10; }
        else if (bulletNum == 2){ yspeed = 10; }
        else if (bulletNum == 3){ xspeed = 10; yspeed = 10; }
        else if (bulletNum == 4){ xspeed = 10; }
        else{ yspeed = 10; }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();

        if (!hit)
        {
            if ( EvaluateCollision().HasFlag(CollisionFlag.ALL) )
            {
                hit = true;
            }
        }
        else if (hitdelay > 0.0f)
        {
            hitdelay = Mathf.Max(hitdelay-1.0f, 0.0f);
        }
        Destroy(gameObject, 0.2f);
    }
}
