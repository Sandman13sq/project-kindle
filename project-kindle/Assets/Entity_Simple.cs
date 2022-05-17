using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Simple : Entity
{
    [SerializeField] private Sprite[] sprites;
    private float image_index, image_speed = 0.2f;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        image_index = Mathf.Repeat(image_index+image_speed*ts, sprites.Length);
        spriterenderer.sprite = sprites[(int)image_index];
    }
}
