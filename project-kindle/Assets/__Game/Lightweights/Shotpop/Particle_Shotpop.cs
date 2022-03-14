using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Shotpop : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriterenderer; 
    [SerializeField] private Sprite[] sprites;
    float lifemax = 6.0f;
    float life = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (life < lifemax)
        {
            spriterenderer.sprite = sprites[(int)(sprites.Length * life/lifemax)];
            life += 1.0f;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
