using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Heart2 : Entity
{
    private Color color1 = new Color(1.0f, 0.0f, 0.0f);
    private Color color2 = new Color(1.0f, 1.0f, 1.0f);

    private float scalestep = 0.0f;

    // Common ======================================================

    // Start is called before the first frame update
    void Start()
    {
        scalestep = Random.Range(0.0f, 1.0f);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "player")
        {
            if (c.gameObject.GetComponent<Entity_Move_Manual>().ChangeHealth(health) != 0)
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Pulse heart
        scalestep = Mathf.Repeat(scalestep + 0.04f, 2.0f*Mathf.PI);
        const float f = 10.0f;
        float amt = Mathf.Max(0.0f, f*(Mathf.Sin(scalestep)-(1.0f-(1.0f/f))));

        spriterenderer.color = Color.Lerp(color1, color2, amt*amt);
        spriterenderer.transform.localScale = new Vector3(
            1.0f+0.1f*amt, 1.0f+0.1f*amt, 1.0f);
    }

    // Method ======================================================

}
