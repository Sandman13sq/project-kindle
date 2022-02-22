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
    private float direction;    // Angle of projectile in degrees

    [SerializeField] float lifemax;
    float life;

    const int LAYER_ENTITY = 6;
    const int LAYER_ENTITY_BIT = 1 << LAYER_ENTITY;

    // Common ============================================================
    
    // Start is called before the first frame update
    void Start()
    {
        life = lifemax;
        
        SetDirection(direction);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();

        // Progress life
        life -= 1.0f;
        if (life <= 0.0)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.layer == LAYER_ENTITY)
        {
            Entity e = c.gameObject.GetComponent<Entity>();

            if (e.isshootable)
            {
                e.ChangeHealth(-damage);
                Destroy(gameObject);
            }
        }
    }

    // Methods ===========================================================

    void UpdateMovement(float ts = 1.0f)
    {
        float directionradians = direction * Mathf.Deg2Rad;

        transform.position = new Vector3(
            transform.position.x + Mathf.Cos(directionradians) * speed * ts,
            transform.position.y + Mathf.Sin(directionradians) * speed * ts,
            transform.position.z
        );
    }

    void SetDirection(float _direction)
    {
        direction = _direction;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, direction);
        spriterenderer.transform.localRotation = transform.localRotation;
    }

    void SetDirection(float _xdir, float _ydir)
    {
        SetDirection( Mathf.Atan2(_ydir, _xdir) );
    }

    void SetSpeed(float _speed)
    {
        speed = _speed;
    }
}
