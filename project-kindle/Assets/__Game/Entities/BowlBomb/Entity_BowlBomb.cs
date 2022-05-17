using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_BowlBomb : Entity
{
    [SerializeField] private Sprite[] sprites;
    private float detonatetime = 100f;
    [SerializeField] private GameObject explosion_prefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        state = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (state == 1)
        {
            detonatetime -= ts;
            if (detonatetime <= 0f)
            {
                //Instantiate(explosion_prefab).transform.position = transform.position;
                Defeat();
                return;
            }

            Vector3 pos = spriterenderer.transform.localPosition;
            spriterenderer.transform.localPosition = new Vector3(0 + Mathf.Sin(detonatetime) * 3f, pos.y, pos.z);

            spriterenderer.sprite = sprites[Mathf.Repeat(detonatetime, 8f) < 4f? 1: 2];
        }

        UpdateDamageShake();
    }

    protected override void OnHealthChange(int change, int weaponprojtype)
    {
        // Hit by dragon breath
        if (state == 0 && weaponprojtype == 1)
        {
            state = 1;
        }

        damageshake = damageshaketime;
    }
}
