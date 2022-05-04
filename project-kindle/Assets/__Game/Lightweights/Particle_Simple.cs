using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Simple : MasterObject
{
    [SerializeField] protected SpriteRenderer spriterenderer; 
    [SerializeField] protected Sprite[] sprites;
    [SerializeField] protected float lifemax = 7.0f;
    protected float life = 0.0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        life = 0.0f;
        spriterenderer.sprite = sprites[0];
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (life < lifemax)
        {
            spriterenderer.sprite = sprites[(int)(sprites.Length * life/lifemax)];
            life += game.TimeStep;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
