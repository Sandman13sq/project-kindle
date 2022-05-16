using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_StorkBag : Entity
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject explosion_prefab;
    float gravity = -0.2f;
    float yspeedterminal = -8f;
    float image_index = 0f;

    // =============================================================================

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Update sprite
        image_index = Mathf.Repeat(game.GetActiveTimeStep() * 0.5f, sprites.Length-1);
        spriterenderer.sprite = sprites[(int)image_index];

        // Fall until terminal speed
        yspeed = Mathf.Max(yspeed + gravity * game.GetActiveTimeStep(), yspeedterminal);

        UpdateMovement();
        CollisionFlag coll = EvaluateCollision(CollisionFlag.DOUBLEY);

        // Create explosion on collision
        if (coll.HasFlag(CollisionFlag.DOWN))
        {
            Defeat();
            return;
        }
    }

    protected override bool OnDefeat()
    {
        Instantiate(explosion_prefab).transform.position = transform.position;
        game.PlaySound("DragonShoot");
        return true;    // Return true to destroy object after
    }
}
