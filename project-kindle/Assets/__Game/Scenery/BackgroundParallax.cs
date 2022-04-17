using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MasterObject
{
    [SerializeField] private SpriteRenderer spriterenderer;
    [SerializeField] private float xspeed = 0.0f;   // Speed to move horizontally
    [SerializeField] private float yspeed = 0.0f;   // Speed to move vertically
    [SerializeField] private float loopwidth, loopheight;
    private Vector3 startingposition;
    private Vector3 campos;

    // Start is called before the first frame update
    void Start()
    {
        startingposition = transform.localPosition;

        var size = spriterenderer.sprite.rect.size;
        spriterenderer.size = new Vector2(
            size[0] * (loopwidth > 0? 8.0f: 1.0f),
            size[1] * (loopheight > 0? 8.0f: 1.0f)
        );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        campos = game.GetCameraPosition();
        var scale = transform.localScale;
        float xx = (loopwidth>0.0f? Mathf.Repeat(campos.x*xspeed, loopwidth*scale.x): (campos.x*xspeed));
        float yy = (loopheight>0.0f? Mathf.Repeat(campos.y*yspeed, loopheight*scale.y): (campos.y*yspeed));
        
        transform.position = new Vector3(
            campos.x - xx,
            campos.y - yy,// + spriterenderer.sprite.rect.height/2.0f,
            startingposition.z
        );
    }
}
