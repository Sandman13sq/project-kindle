using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Popsmoke : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriterenderer;
    private int spritecount;
    private int spriteindex;

    [SerializeField] private float lifemax = 16.0f;
    private float life;
    private float xspeed, yspeed;

    // Start is called before the first frame update
    void Start()
    {
        lifemax *= Random.Range(0.8f, 1.2f);
        life = lifemax;

        float size = Random.Range(1.0f, 1.6f);
        spriterenderer.transform.localScale = new Vector3(size, size, 1.0f);

        xspeed = Random.Range(-4.0f, 4.0f);
        yspeed = Random.Range(-4.0f, 4.0f);

        spritecount = sprites.Length;
        spriteindex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (life > 0.0f)
        {
            life -= 1.0f;

            if (spritecount <= 1)
            {
                spriteindex = 0;
            }
            else
            {
                spriteindex = (int)(spritecount*(1.0f-life/lifemax));
                if (spriteindex < 0) {spriteindex = 0;}
                else if (spriteindex >= spritecount) {spriteindex = spritecount-1;}
            }

            spriterenderer.sprite = sprites[spriteindex];
        }
        else
        {
            Destroy(gameObject);
        }

        transform.position = new Vector3(
            transform.position.x + xspeed,
            transform.position.y + yspeed,
            transform.position.z
        );
    }
}
