using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MasterObject
{
    [SerializeField] private float xspeed = 0.0f;   // Speed to move horizontally
    [SerializeField] private float yspeed = 0.0f;   // Speed to move vertically
    [SerializeField] private float loopwidth, loopheight;
    private Vector3 startingposition;
    private Vector3 campos;

    // Start is called before the first frame update
    void Start()
    {
        startingposition = transform.localPosition;
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        campos = game.GetCameraPosition();
        var scale = transform.localScale;
        transform.localPosition = startingposition - new Vector3(
            (loopwidth>0.0f? Mathf.Repeat(campos.x*xspeed, loopwidth*scale.x): (campos.x*xspeed)),
            (loopheight>0.0f? Mathf.Repeat(campos.y*yspeed, loopheight*scale.y): (campos.y*yspeed)),
            0.0f
        );
    }
}
