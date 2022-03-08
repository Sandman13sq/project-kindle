using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Move_Manual : Entity
{
	// Variables =========================================

	private const float floory = 136.0f;

	public bool jumpheld;
	public float jumpbuffer;
    private float jumpbuffertime = 7.0f; // Max number of frames ahead of time where a jump press will still be read

	[SerializeField] private PlayerData playerdata;	// Holds health, energy, level, etc.
	[SerializeField] private GameObject projectile;	// Temporary until we find a better way that doesn't require the inspector
	[SerializeField] private Sprite[] sprites;
	private int spriteindex;
	private int recentshot;

	private float hsign;    // Horizontal sign. {-1, 1}
    private float vsign;    // Vertical sign. {-1, 0, 1} 

	private float iframes = 0.0f;	// Frames of invincibility after taking damage
	private float iframestime = 150.0f;

	[SerializeField] private Weapon weapon;

	// Common ===============================================================
	
	// Start is called before the first frame update
	void Start()
	{
		hsign = 1.0f;
		recentshot = 100;
		Application.targetFrameRate = 60; // Temporary. Will remove later
		playerdata.SetHealth(health);
		weapon.SetPlayer(this);
	}

	// Update is called once per frame
	async void Update()
	{
		float moveacceleration = 0.3f;
		float movedeceleration = 0.5f;
		float moveaccelerationair = 0.2f;
		float movespeedmax = 4.0f;
		float jumpstrength = 4.5f;
		float gravity = -0.24f;
		float gravityjump = -0.10f;

		// Grab input values
		float xlev = Input.GetAxisRaw("Horizontal");
		float ylev = Input.GetAxisRaw("Vertical");
		float lastvsign = vsign;
		float lasthsign = hsign;

		// Flip sprite if moving left. If shift is held, lock direction
        if (xlev != 0.0 && !Input.GetKey(KeyCode.LeftShift))
        {
			hsign = (xlev > 0.0f)? 1.0f: -1.0f;
            spriterenderer.flipX = xlev < 0.0f;
        }
		vsign = ylev;

		// Jump buffer
        if (jumpbuffer >= 0.0f)
        {
            jumpbuffer -= 1.0f;
        }

		if (Input.GetButton("Jump"))
        {
            jumpbuffer = jumpbuffertime;
        }

		// Ground Movement
		/*
			When player is moving in the same direction as the input, use acceleration and approach movespeed.
			When player is moving in the opposing direction as the input, use deceleration and approach movespeed.
			When no input is held, use deceleration and approach 0.
		*/
		if (onground)
		{
			if (xlev > 0.0f) // Moving Right
			{
				xspeed = Mathf.Min(xspeed + (xlev==Mathf.Sign(xspeed)? moveacceleration: movedeceleration), movespeedmax);
			}
			else if (xlev < 0.0f)   // Moving Left
			{
				xspeed = Mathf.Max(xspeed - (xlev==Mathf.Sign(xspeed)? moveacceleration: movedeceleration), -movespeedmax);
			}
			else    // No input held
			{
				if (Mathf.Abs(xspeed) <= movedeceleration) // Clamp to 0 when speed is small enough
				{
					xspeed = 0.0f;
				}
				else    // Approach 0 with deceleration
				{
					xspeed -= Mathf.Sign(xspeed) * movedeceleration;
				}
			}
			
			// Jump
            if ( jumpbuffer > 0.0f )
            {
				yspeed += jumpstrength;
				jumpbuffer = 0.0f;
				jumpheld = true;
			}
		}
		// In Air
		else
		{
			jumpheld = jumpheld && Input.GetButton("Jump") && yspeed > 0.0f;
			if (xlev > 0.0f) // Moving Right
			{
				xspeed = Mathf.Min(xspeed + moveaccelerationair, movespeedmax);
			}
			else if (xlev < 0.0f)   // Moving Left
			{
				xspeed = Mathf.Max(xspeed - moveaccelerationair, -movespeedmax);
			}
			
			// Apply gravity
			yspeed = Mathf.Max(yspeed+(jumpheld? gravityjump: gravity), -8.0f);
		}

		// Update speeds
		UpdateMovement();

		// Collision
		CollisionFlag collisionresult = EvaluateCollision(
			CollisionFlag.CHANGESPEED | CollisionFlag.DOUBLEX);
		if ( collisionresult.HasFlag(CollisionFlag.DOWN) )
		{
			yspeed = Mathf.Max(yspeed, 0.0f);
		}

		// Temporary Spriting
		recentshot += 1;

		if (
			(lastvsign != vsign && vsign == 0.0f) ||
			(lasthsign != hsign && recentshot > 10)
		)
		{
			recentshot = 100;
		}

		if (ylev > 0)
		{
			spriterenderer.sprite = sprites[recentshot < 5? 4: 5];
		}
		else
		{
			if (recentshot < 5)
			{
				spriterenderer.sprite = sprites[7];
			}
			else if (recentshot < 50)
			{
				spriterenderer.sprite = sprites[6];
			}
			else
			{
				spriterenderer.sprite = sprites[(recentshot / 10) % 4];
				spriteindex = (spriteindex + 1) % 4;
			}
		}

		// Hitbox collision
		if (hitcollider)
		{
			RaycastHit2D[] hitresults = new RaycastHit2D[8];
			ContactFilter2D filter = new ContactFilter2D();
			filter.layerMask = LAYER_HITBOX_BIT;
			
			hitcollider.Cast(
				new Vector2(0.0f, 0.0f),
				filter,
				hitresults,
				0.0f
			);

			// Iterate through hits
			foreach (RaycastHit2D hit in hitresults)
			{
				if (hit.collider != null)
				{
					if (hit.collider.gameObject.TryGetComponent(out ParentEntity p))
					{
						Entity e = p.GetEntity();
						DoDamage(e.GetAttack());
						break;
					}
					
				}
			}
		}

		// Iframes
		if (iframes > 0.0f)
		{
			iframes = Mathf.Max(0.0f, iframes-1.0f);

			if (iframes > 0.0f)
			{
				spriterenderer.enabled = Mathf.Repeat(iframes, 8.0f) < 4.0f;
			}
			else
			{
				spriterenderer.enabled = true;
			}
		}
	}

	// Methods ===============================================================

	public PlayerData GetPlayerData() {return playerdata;}

	public void OnShoot()
	{
		recentshot = 0;
	}

	public override int ChangeHealth(int value)
	{
		if (value >= 0 || iframes == 0.0f)
		{
			int healthdiff = base.ChangeHealth(value);
			playerdata.SetHealth(health); // Update HUD

			// Subtract energy when health is lost
			if (healthdiff < 0)
			{
				weapon.AddEnergy(healthdiff);
			}
			// Flash when health is gained
			else
			{
				playerdata.HealthFlashMeter();
			}
			
			return healthdiff;
		}
		else
		{
			return 0;
		}
		

	}

	protected override void OnHealthChange(int diff)
	{
		if (diff < 0)
		{
			iframes = iframestime;
			yspeed = 5.0f;
			jumpheld = true;
		}
	}

	public int AddEnergy(int _energy)
	{
		return weapon.AddEnergy(_energy);
	}

	// Utility ================================================================

	public float GetHSign() {return hsign;}
	public float GetVSign() {return vsign;}
	public bool GetOnGround() {return onground;}
}
