using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Energy0 : Entity
{
    private float gravity = -0.16f;
    private float terminalvelocity = -8.0f;
    private float colorstep = 0.0f;

    private Color color1 = new Color(0.5f, 1.0f, 1.0f);
    private Color color2 = new Color(1.0f, 1.0f, 1.0f);

    private const float attractspeed = 0.02f; // Speed that energy will move towards player
    private const float attractrange = 64.0f; // Range that energy will start moving towards player

    private float lifemax = 360.0f;
    private float life;

    // Start is called before the first frame update
    protected override void Start()
    {
        life = lifemax;
        colorstep = Random.Range(0.0f, 1.0f);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "player")
        {
            int leftoverenergy = game.GetPlayerData().AddEnergy(energy);
            energy = leftoverenergy;
            if (energy == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Update color
        colorstep = Mathf.Repeat(colorstep + 0.02f, Mathf.PI);
        spriterenderer.color = Color.Lerp(color1, color2, Mathf.Sin(colorstep*colorstep));
        
        // Move
        if (yspeed > terminalvelocity)
        {
            yspeed = Mathf.Max(yspeed+gravity, terminalvelocity);
        }

        // If ground collision...
        if ( (EvaluateCollision() & CollisionFlag.DOWN ) != 0 )
        {
            // Dampen speed on each bounce
            yspeed = Mathf.Max( Mathf.Abs(yspeed) * 0.8f, 3.0f);
            xspeed *= 0.8f;
        };

        // Move towards player if close
        Physics2D.CircleCast(
            new Vector2(transform.position.x, transform.position.y),
            attractrange,
            new Vector2(0.0f, 0.0f),
            new ContactFilter2D() {layerMask=~LAYER_WORLD_BIT},
            castresults,
            attractrange
        );

        foreach (var hit in castresults)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "player")
                {
                    xspeed += attractspeed * Mathf.Sign(
                        hit.collider.gameObject.transform.position.x-transform.position.x);
                }
            }
        }

        UpdateMovement();

        // Destroy after life timer runs out
        if (lifemax > 0.0f)
        {
            if (life > 0.0f)
            {
                life -= 1.0f;
                
                // Sprite flicker
                spriterenderer.enabled = life > (lifemax/3.0f) || Mathf.Repeat(life, 6.0f) < 3.0f;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
