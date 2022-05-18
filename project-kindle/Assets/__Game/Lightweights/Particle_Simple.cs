using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Simple : MasterObject
{
    [SerializeField] protected float zstart = -9f;
    [SerializeField] protected SpriteRenderer spriterenderer; 
    [SerializeField] protected Sprite[] sprites;
    [SerializeField] protected float lifemax = 7.0f;
    protected float life = 0.0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        life = 0.0f;
        spriterenderer.sprite = sprites[0];
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            zstart
        );
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
