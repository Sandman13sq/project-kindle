using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DmrMath;

public class Entity_Rattah : Entity
{
    enum State
    {
        idle,
        chase,
        attack0,
        attack1,
        attack2,
    }

    [SerializeField] private Sprite[] sprites_idle;
    [SerializeField] private Sprite[] sprites_attack;
    float image_index;
    float statestep;

    // Start is called before the first frame update
    protected override void Start()
    {
        state = (int)State.idle;
        image_index = 0.0f;
        statestep = 0.0f;

        hitboxcollider.enabled = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        const float chaserange = 400.0f;
        const float ignorerange = chaserange * 1.5f;
        const float attackrange = 64.0f;
        const float movespeed = 2.8f;
        const float moveacc = 0.13f;
        const float jumpheight = 4.0f;

        var p = game.GetPlayer();

        yspeed -= ts*0.2f; // Gravity

        UpdateDamageShake();
        UpdateMovement();
        var collflag = EvaluateCollision(CollisionFlag.CHANGESPEED | CollisionFlag.DOUBLEX);
        onground = collflag.HasFlag(CollisionFlag.DOWN);

        switch((State)state)
        {
            // ==========================================================
            case(State.idle):
                if (onground) {xspeed = ApproachTS(xspeed, 0.0f, moveacc);}
                image_index = Mathf.Repeat(image_index + ts/6.0f, sprites_idle.Length);
                spriterenderer.sprite = sprites_idle[(int)image_index];
                hitboxcollider.enabled = false;

                // Transition to chase
                if (DistanceTo(p) <= chaserange)
                {
                    state = (int)State.chase;
                }
                
                break;
            
            // =========================================================
            case(State.chase):
                xspeed = ApproachTS(xspeed, movespeed*SignToX(p), moveacc * (onground? 1.0f: 0.9f));
                
                image_index = Mathf.Repeat(image_index + ts/6.0f, sprites_idle.Length);
                spriterenderer.sprite = sprites_idle[(int)image_index];
                spriterenderer.flipX = SignToX(p) < 0.0f;
                
                // Jump over walls when on ground and touching a wall
                if (
                    onground &&
                    (collflag.HasFlag(CollisionFlag.LEFT) || collflag.HasFlag(CollisionFlag.RIGHT)))
                {
                    yspeed = jumpheight;
                }

                // Go back to idle when Kindle is out of range
                if (DistanceTo(p) >= ignorerange)
                {
                    state = (int)State.idle;
                }

                // Attack when Kindle is close. Range increases if moving
                if (DistanceTo(p) <= attackrange + Mathf.Abs(xspeed)*10.0f)
                {
                    state = (int)State.attack0;
                    image_index = 0.0f;
                    statestep = 0.0f;
                }
                
                break;

            
            // =========================================================
            case(State.attack0):
                if (onground) {xspeed = ApproachTS(xspeed, 0.0f, moveacc);}
                statestep += ts;
                spriterenderer.sprite = sprites_attack[0];

                if (statestep >= 20.0f)
                {
                    game.PlaySound("RattahMiss");
                    state = (int)State.attack1;
                    statestep = 0.0f;
                }
                break;
            
            // =========================================================
            // Hitbox state
            case(State.attack1):
                if (onground) {xspeed = ApproachTS(xspeed, 0.0f, moveacc);}
                spriterenderer.sprite = sprites_attack[1];

                // Move to next state
                if (statestep >= 6.0f)
                {
                    state = (int)State.attack2;
                    statestep = 0.0f;
                    
                    hitboxcollider.enabled = false;
                }
                else
                {
                    // Move hitbox in front of where Rattah is facing
                    var pos = hitboxcollider.transform.localPosition;
                    hitboxcollider.transform.localPosition = new Vector3(
                        Mathf.Abs(pos.x) * Polarize(!spriterenderer.flipX), pos.y, pos.z
                    );

                    hitboxcollider.enabled = true;
                }

                statestep += ts;
                break;
            
            // =========================================================
            case(State.attack2):
                if (onground) {xspeed = ApproachTS(xspeed, 0.0f, moveacc);}
                spriterenderer.sprite = sprites_attack[2];

                // Return to idle state
                if (statestep >= 40.0f)
                {
                    state = (int)State.idle;
                    statestep = 0.0f;
                }

                statestep += ts;
                break;
        }
    }
    
    protected override bool OnDefeat()
    {
        base.OnDefeat();
        hitboxcollider.enabled = false;

        return true;
    }

    protected override void OnHealthChange(int diff)
    {
        base.OnHealthChange(diff);

        if (diff < 0)
        {
            xspeed *= 0.2f;
        }
    }
}
