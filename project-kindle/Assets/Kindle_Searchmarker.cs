using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kindle_Searchmarker : MasterObject
{
    [SerializeField] SpriteRenderer spriterenderer;
    private float sprite_y;
    private float rotatestep = 0.0f;
    private float rotatespeed = 0.1f;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        sprite_y = spriterenderer.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Update marker
        if (
            !game.GameFlagGet(GameFlag.lock_player) &&
            !game.EventIsRunning() &&
            active
            )
        {
            spriterenderer.enabled = true;
            spriterenderer.transform.localScale = new Vector2(Mathf.Sin(rotatestep), 1.0f);
            spriterenderer.transform.localPosition = new Vector3(0f, sprite_y+2f*Mathf.Sin(rotatestep), 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, 2f*Mathf.Sin(rotatestep * 0.5f));
            rotatestep += rotatespeed;
        }
        // No marker
        else
        {
            spriterenderer.enabled = false;
        }
    }

    public void SetActive(bool _active)
    {
        active = _active;
    }
}
