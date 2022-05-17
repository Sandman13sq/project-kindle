using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_HealthPack : Entity
{
    [SerializeField] Sprite[] sprites;

    private float image_speed = 0.2f;
    private float image_index = 0f;
    private float gravity = -0.3f;
    private float terminal = -6f;

    // Start is called before the first frame update
    protected override void Start()
    {
        image_index = Random.value * sprites.Length;
    }

    // Update is called once per frame
    protected override void Update()
    {
        transform.position += new Vector3(xspeed, yspeed, 0f) * ts;

        var cflag = EvaluateCollision(CollisionFlag.DOUBLEX);
        if (cflag.HasFlag(CollisionFlag.DOWN))
        {
            // Stay on ground
            if (Mathf.Abs(yspeed) <= 0.1f)
            {
                yspeed = 0f;
            }
            // Bounce
            else
            {
                yspeed = Mathf.Abs(yspeed) * 0.6f;
            }
        }
        else
        {
            yspeed = Mathf.Max(yspeed + ts*gravity, terminal);
        }
        
        image_index = Mathf.Repeat(image_index + image_speed * ts, sprites.Length);
        spriterenderer.sprite = sprites[(int)image_index];
    }
}
