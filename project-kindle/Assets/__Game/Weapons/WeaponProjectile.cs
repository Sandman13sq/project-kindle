using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Projectile fired from player weapon
public class WeaponProjectile : MonoBehaviour
{
    public SpriteRenderer spriterenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float image_index = 0.0f;
    [SerializeField] private float image_speed = 0.0f;
    public Collider2D worldcollider;    // Used to interact with the world
    public Collider2D hitboxcollider;  // Used for hitbox collisions
    public Rigidbody2D rbody;  // Necessary for collision detection

    [SerializeField] private float speed;
    [SerializeField] private int damage;

    [SerializeField] float lifemax;
    float life;

    [SerializeField] private Weapon sourceweapon = null; // Weapon component that the projectile was fired from

    protected const int LAYER_WORLD = 3;
    protected const int LAYER_WORLD_BIT = 1 << LAYER_WORLD;
    protected const int LAYER_ENTITY = 6; 
    protected const int LAYER_ENTITY_BIT = 1 << LAYER_ENTITY;
    protected const int LAYER_HITBOX = 7; 
    protected const int LAYER_HITBOX_BIT = 1 << LAYER_HITBOX;
    protected const int LAYER_HURTBOX = 8; 
    protected const int LAYER_HURTBOX_BIT = 1 << LAYER_HURTBOX;

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

        // Enemy Collision
        if (hitboxcollider)
        {
            RaycastHit2D[] hitresults = new RaycastHit2D[8];
            CastHitbox(hitresults);

            foreach (RaycastHit2D hit in hitresults)
            {
                Entity e = GetEntityFromCollider(hit.collider);
                
                if (e = GetEntityFromCollider(hit.collider))
                {
                    // Entity has the shootable flag set
                    if (e.isshootable)
                    {
                        e.ChangeHealth(-damage);
                        DecrementShotCount();
                        Destroy(gameObject);
                        return;
                    }
                }

                
            }
        }

        // Update sprite if array is populated
        if (sprites.Length > 0)
        {
            spriterenderer.sprite = sprites[Mathf.FloorToInt(Mathf.Clamp(image_index, 0.0f, sprites.Length-1))];
            image_index = Mathf.Repeat(image_index+image_speed, sprites.Length);
        }

        // Progress life
        life -= 1.0f;
        if (life <= 0.0)
        {
            DecrementShotCount();
            Destroy(gameObject);
            return;
        }
    }
    
    void OnCollisionEnter2D(Collision2D c)
    {
        // Interact with world
        if (c.gameObject.layer == LAYER_WORLD)
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

    public void SetSpeed(float _speed) {speed = _speed;}
    public void AddSpeed(float _speed) {speed += _speed;}
    public void MultiplySpeed(float _speed) {speed *= _speed;}

    public void SetSourceWeapon(Weapon _weapon)
    {
        sourceweapon = _weapon;
    }

    // Casts hitbox against hurtboxes and populates results in given variable. Returns number of results
    protected int CastHitbox(RaycastHit2D[] hitresults)
    {
        if (!hitboxcollider) {return 0;}
        
        hitboxcollider.Cast(
            new Vector2(0.0f, 0.0f),
            new ContactFilter2D() {layerMask=LAYER_HURTBOX_BIT},
            hitresults,
            0.0f
        );

        return hitresults.Length;
    }

    // Returns root entity from collider if exists, null otherwise.
    protected Entity GetEntityFromCollider(Collider2D c)
    {
        if (c != null)
        {
            if (c.gameObject.TryGetComponent(out ParentEntity p))
            {
                return p.GetEntity();
            }

            if (c.gameObject.TryGetComponent(out Entity e))
            {
                return e;
            }
        }
        
        return null;
    }
}
