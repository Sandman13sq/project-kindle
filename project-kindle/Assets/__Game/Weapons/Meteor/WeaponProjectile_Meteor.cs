using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using static DmrMath;

public class WeaponProjectile_Meteor : WeaponProjectile
{
    private float xspeed, yspeed;
    private float gravity = -0.2f;
    private float radius;
    private int explosiondamage;

    RaycastHit2D[] castresults = new RaycastHit2D[8];
    [SerializeField] CircleCollider2D explosionradius;

    void Start()
    {
        life = lifemax;
        float radians = transform.localRotation.eulerAngles[2] * Mathf.Deg2Rad;
        xspeed = Mathf.Cos(radians) * speed + Mathf.Sin(radians);
        yspeed = Mathf.Sin(radians) * speed;
        radius = (worldcollider as CircleCollider2D).radius;
        explosiondamage = damage;
        damage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Gravity
        yspeed += gravity;

        xspeed = Mathf.Clamp(xspeed, -6f, 6f);
        yspeed = Mathf.Clamp(yspeed, -8f, 8f);

        transform.position = new Vector3(
            transform.position.x + xspeed * game.TimeStep,
            transform.position.y + yspeed * game.TimeStep,
            transform.position.z
        );

        // Bounce off world
        Array.Clear(castresults, 0, castresults.Length);
        worldcollider.Cast(
            new Vector2(0.0f, 0.0f),
            new ContactFilter2D() {layerMask=LAYER_WORLD_BIT, useLayerMask=true},
            castresults,
            0.0f,
            true
        );
        
        foreach(var hit in castresults)
        {
            if (hit.collider != null)
            {
                var spd = PointDistance(0f, 0f, xspeed, yspeed);
                if (Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y))
                {
                    xspeed = hit.normal.x * speed;
                    yspeed += hit.normal.y * spd;
                }
                else
                {
                    xspeed += hit.normal.x * spd;
                    yspeed = hit.normal.y * speed;
                }
                
                // Move out of collision
                transform.position = new Vector3(
                    transform.position.x + hit.normal.x * radius,
                    transform.position.y + hit.normal.y * radius,
                    transform.position.z
                );
            }
        }

        CastForEnemy(hitboxcollider);

        UpdateSprite();
        UpdateLife();
    }

    protected void OnEnemyHit(Entity e)
    {
        MeteorExplosion explosion = Instantiate(obj_on_hit).GetComponent<MeteorExplosion>();
        explosion.transform.position = transform.position;
        explosion.SetRadius(explosionradius.radius);
        explosion.SetDamage(explosiondamage);

        Destroy(gameObject);
    }

    protected void OnExpire()
    {
        MeteorExplosion explosion = Instantiate(obj_on_hit).GetComponent<MeteorExplosion>();
        explosion.transform.position = transform.position;
        explosion.SetRadius(explosionradius.radius);
        explosion.SetDamage(explosiondamage);

        Destroy(gameObject);
    }
}
