using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Heart2 : Entity
{
    private Color color1 = new Color(1.0f, 0.0f, 0.0f);
    private Color color2 = new Color(1.0f, 1.0f, 1.0f);

    private float scalestep = 0.0f;
    private float life;
    [SerializeField] private float lifemax;
    [SerializeField] private GameObject healpulse_prefab;

    // Common ======================================================

    // Start is called before the first frame update
    protected override void Start()
    {
        scalestep = Random.Range(0.0f, 1.0f);
        life = lifemax;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "player")
        {
            if (c.gameObject.GetComponent<Entity_Player>().ChangeHealth(health) != 0)
            {
                GameObject o = Instantiate(healpulse_prefab);
                o.transform.position = transform.position;
                o.transform.localScale *= UnityEngine.Random.Range(0.9f, 1.0f);

                Defeat();
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Progress Life
        if (life > 0f) {life -= ts;}
        else
        {
            Defeat();
            return;
        }

        // Flicker
        if (life < lifemax*0.3)
        {
            spriterenderer.enabled = DmrMath.BoolStep(life, 4f);
        }

        // Pulse heart
        scalestep = Mathf.Repeat(scalestep + 0.04f*ts, 2.0f*Mathf.PI);
        const float f = 10.0f;
        float amt = Mathf.Max(0.0f, f*(Mathf.Sin(scalestep)-(1.0f-(1.0f/f))));

        spriterenderer.color = Color.Lerp(color1, color2, amt*amt);
        spriterenderer.transform.localScale = new Vector3(
            1.0f+0.1f*amt, 1.0f+0.1f*amt, 1.0f);
    }

    // Method ======================================================

}
