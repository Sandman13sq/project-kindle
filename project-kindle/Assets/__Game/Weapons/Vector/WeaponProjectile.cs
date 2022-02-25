using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Projectile fired from player weapon
public class WeaponProjectile : MonoBehaviour
{
    public SpriteRenderer spriterenderer;
    public Collider2D worldcollider;    // Used to interact with the world
    public Collider2D hitcollider;  // Used for hitbox collisions
    public Rigidbody2D rigidbody;  // Necessary for collision detection

    [SerializeField] private float speed;
    [SerializeField] private int damage;

    [SerializeField] float lifemax;
    float life;

    [SerializeField] private Weapon sourceweapon = null; // Weapon component that the projectile was fired from

    const int LAYER_WORLD = 3;
    const int LAYER_WORLD_BIT = 1 << LAYER_WORLD;
    const int LAYER_ENTITY = 6;
    const int LAYER_ENTITY_BIT = 1 << LAYER_ENTITY;

    // Common ============================================================
    
    // Start is called before the first frame update
    void Start()
    {
        life = lifemax;
        SetDirectionDeg(transform.localRotation.eulerAngles[2]);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();

        // Progress life
        life -= 1.0f;
        if (life <= 0.0)
        {
            DecrementShotCount();
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D c)
    {
        // Interact with entity
        if (c.gameObject.layer == LAYER_ENTITY)
        {
            Entity e = c.gameObject.GetComponent<Entity>();

            // Entity has the shootable flag set
            if (e.isshootable)
            {
                e.ChangeHealth(-damage);
                DecrementShotCount();
                Destroy(gameObject);
            }
        }
        // Interact with world
        else if (c.gameObject.layer == LAYER_WORLD)
        {
            DecrementShotCount();
            Destroy(gameObject);
        }
    }

    void DecrementShotCount()
    {
        if (sourceweapon != null)
        {
            sourceweapon.DecrementShotCount();
        }
    }

    // Methods ===========================================================

    // Update position
    void UpdateMovement(float ts = 1.0f)
    {
        float directionradians = transform.localRotation.eulerAngles[2] * Mathf.Deg2Rad;

        transform.position = new Vector3(
            transform.position.x + Mathf.Cos(directionradians) * speed * ts,
            transform.position.y + Mathf.Sin(directionradians) * speed * ts,
            transform.position.z
        );
    }

    // Set projectile direction using radians
    public void SetDirectionRad(float _direction, float _right = 0.0f)
    {
        SetDirectionDeg(Mathf.Rad2Deg*_direction, _right);
    }

    // Set projectile direction using degrees
    public void SetDirectionDeg(float _direction, float _right = 0.0f)
    {
        // Flip y based on direction
        float cos = Mathf.Cos(_direction*Mathf.Deg2Rad);
        if (Mathf.Abs(cos) > 0.01f)
        {
            spriterenderer.flipY = cos < 0.0f;
        }
        else if (Mathf.Abs(_right) > 0.01f)
        {
            spriterenderer.flipY = _right < 0.0f;
        }

        // Update transforms
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, _direction);
        spriterenderer.transform.localRotation = transform.localRotation;
    }

    public void SetDirectionXY(float _xdir, float _ydir)
    {
        SetDirectionRad( Mathf.Atan2(_ydir, _xdir) );
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    public void SetSourceWeapon(Weapon _weapon)
    {
        sourceweapon = _weapon;
    }
}
