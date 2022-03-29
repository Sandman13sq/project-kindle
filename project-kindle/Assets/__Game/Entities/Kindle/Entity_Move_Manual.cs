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
	//stuff for animation:
	[SerializeField] private Animator animator;

	private bool aimingUp; //bool to check if kindle is aiming up

	private float hsign;    // Horizontal sign. {-1, 1}
    private float vsign;    // Vertical sign. {-1, 0, 1} 

	private float iframes = 0.0f;	// Frames of invincibility after taking damage
	private float iframestime = 150.0f;

	private int weaponindex = 0;
	[SerializeField] private Weapon[] weapons;
	private Weapon activeweapon;

	// Common ===============================================================
	
	// Start is called before the first frame update
	void Start()
	{
		hsign = 1.0f;
		Application.targetFrameRate = 60; // Temporary. Will remove later
		playerdata.SetHealth(health);

		foreach (var w in weapons)
		{
			w.SetPlayer(this);
		}

		SetActiveWeapon(1);
	}

	// Update is called once per frame
	void Update()
	{
		animator.SetFloat("Speed", Mathf.Abs(xspeed));//set animator parameter to xspeed

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

		if (Input.GetButtonDown("Jump"))
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

		// Aiming up and down
		if(ylev > 0)
		{
			aimingUp = true;
			animator.SetBool("AimingUp", aimingUp);
		}

		else
		{
			aimingUp = false;
			animator.SetBool("AimingUp", aimingUp);
		}

		// Hitbox collision
		if (hurtboxcollider)
		{
			RaycastHit2D[] hitresults = new RaycastHit2D[8];
			CastHurtbox(hitresults);

			// Iterate through hits
			foreach (RaycastHit2D hit in hitresults)
			{
				Entity e = GetEntityFromCollider(hit.collider);
				if (e)
				{
					// Do damage from hitbox
					DoDamage(e.GetAttack());
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

		// Switch Weapon
		if (Input.GetButtonDown("WeaponNext"))
		{
			weaponindex = (weaponindex+1) % 2;
			SetActiveWeapon(weaponindex);
		}
		
		if (Input.GetButtonDown("WeaponPrev"))
		{
			weaponindex = weaponindex==0? 1: weaponindex-1;
			SetActiveWeapon(weaponindex);
		}

		//turn off shootingside when fire button is let go
		if (Input.GetButtonUp("Fire1"))
		{
			animator.SetBool("ShootingUp", false);
			animator.SetBool("ShootingSide", false);
		}
	}

	// Methods ===============================================================

	public PlayerData GetPlayerData() {return playerdata;}

	//Can check for animation stuff in OnShoot
	public void OnShoot(){
		if(aimingUp){
			animator.SetBool("ShootingUp", true);
		}

		else{
			animator.SetBool("ShootingSide", true);
		}
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
				activeweapon.AddEnergy(healthdiff);
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
		return activeweapon.AddEnergy(_energy);
	}

	public void SetActiveWeapon(int index)
	{
		weaponindex = index;
		activeweapon = weapons[weaponindex];

		foreach (var w in weapons)
		{
			w.SetActive(false);
		}

		activeweapon.SetActive(true);

		playerdata.SetWeapon(weaponindex);
	}

	// Utility ================================================================

	public float GetHSign() {return hsign;}
	public float GetVSign() {return vsign;}
	public bool GetOnGround() {return onground;}
}
