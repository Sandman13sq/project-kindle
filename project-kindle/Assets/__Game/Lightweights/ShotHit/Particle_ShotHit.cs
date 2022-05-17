using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_ShotHit : MasterObject
{
    [SerializeField] private SpriteRenderer spriterenderer; 
    [SerializeField] private Sprite[] sprites;
    float lifemax = 5.0f;
    float life = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            -9f
        );
    }

    // Update is called once per frame
    void Update()
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
