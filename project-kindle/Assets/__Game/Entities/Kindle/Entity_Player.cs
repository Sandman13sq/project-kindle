using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Player : Entity
{
	enum State
	{
		control,
		defeat,
	}

	// Variables =========================================
	private const float floory = 136.0f;

	private bool jumpheld;
	private float jumpbuffer;
    private float jumpbuffertime = 7.0f; // Max number of frames ahead of time where a jump press will still be read

	private PlayerData playerdata;	// Holds health, energy, level, etc.
	//======= stuff for animation:
	[SerializeField] Animator animator;

	private bool aimingUp; //bool to check if kindle is aiming up
	private bool aimingDown; //bool to check if kindle is aiming down

	private int ticks = 0; //used to force shooting animation to play for a good bit

	//====================================
	private float hsign;    // Horizontal sign. {-1, 1}
    private float vsign;    // Vertical sign. {-1, 0, 1} 

	private float iframes = 0.0f;	// Frames of invincibility after taking damage
	private float iframestime = 150.0f;
	private bool showplayer;

	[SerializeField] private GameObject gameover_prefab;

	public bool defeat;

	// Movement constants -------------------------------
	float moveacceleration = 0.4f;
	float movedeceleration = 0.6f;
	float moveaccelerationair = 0.25f;
	float movespeedmax = 5.0f;
	float jumpstrength = 5.5f;
	float gravity = -0.26f;
	float gravityjump = -0.12f;

	// Common ===============================================================
	
	// Called on creation
	void Awake()
    {
        game.SetPlayer(this);
    }

	// Start is called before the first frame update
	protected override void Start()
	{
		hsign = 1.0f;

		playerdata = game.GetPlayerData();
		playerdata.SetHealth(health, healthmax);

		showplayer = true;

		state = (int)State.control;
		animator.SetBool("Defeat", false);
	}

	// Update is called once per frame
	protected override void Update()
	{
		// Used to reset shooting animation
		if(ticks < 30)
		{
			ticks += 1;
		}
		else if(ticks == 30)
		{
			animator.SetBool("ShootingUp", false);
			animator.SetBool("ShootingSide", false);
		}
		
		animator.SetFloat("Speed", Mathf.Abs(xspeed)); //set animator parameter to xspeed

		// Grab input values
		float xlev = Input.GetAxisRaw("Horizontal");	// Left/Right player input
		float ylev = Input.GetAxisRaw("Vertical");		// Up/Down player input
		float lastvsign = vsign;
		float lasthsign = hsign;
		bool controlslocked = game.GameFlagGet(_GameHeader.GameFlag.lock_player);

		float grav = gravity;
		
		switch((State)state)
		{
			// -------------------------------------------------------------------
			// Control State - Player moves the character
			case(State.control): {
				animator.SetBool("Defeat", false);

				// Use controls if controls are free
				if ( !controlslocked )
				{
					// Flip sprite if moving left. If shift is held, lock direction
					if (xlev != 0.0 && !Input.GetKey(KeyCode.LeftShift))
					{
						hsign = (xlev > 0.0f)? 1.0f: -1.0f;
						spriterenderer.flipX = xlev < 0.0f;
					}
					vsign = ylev;
					
					// Jump buffer
					/*
						As long as the jump buffer value is non-zero
						A jump will occur at the next possible frame when player is on ground
					*/
					if (jumpbuffer >= 0.0f)
					{
						jumpbuffer -= 1.0f;	// Decrement buffer time
					}

					if (Input.GetButtonDown("Jump"))
					{
						jumpbuffer = jumpbuffertime; // Reset jump buffer
					}

					// Switch Weapon
					if (Input.GetButtonDown("WeaponNext"))
					{
						playerdata.NextWeapon();
					}
					
					if (Input.GetButtonDown("WeaponPrev"))
					{
						playerdata.PrevWeapon();
					}
				}
				else
				{
					xlev = 0.0f;
					ylev = 0.0f;
				}

				// Ground Movement
				/*
					When player is moving in the same direction as the input, use acceleration and approach movespeed.
					When player is moving in the opposing direction as the input, use deceleration and approach movespeed.
					When no input is held, use deceleration and approach 0.
				*/
				if (onground)
				{
					yspeed = Mathf.Max(0.0f, yspeed); // Keep upwards movement, if any

					// When current speed and input direction agree, use acceleration, else use deceleration
					if (xlev > 0.0f) // Moving Right
					{
						xspeed = Mathf.Min(xspeed + ts * (xlev==Mathf.Sign(xspeed)? moveacceleration: movedeceleration), movespeedmax);
					}
					else if (xlev < 0.0f)   // Moving Left
					{
						xspeed = Mathf.Max(xspeed - ts * (xlev==Mathf.Sign(xspeed)? moveacceleration: movedeceleration), -movespeedmax);
					}
					else    // No input held
					{
						if (Mathf.Abs(xspeed) <= movedeceleration) // Clamp to 0 when speed is small enough
						{
							xspeed = 0.0f;
						}
						else    // Approach 0 with deceleration
						{
							xspeed -= Mathf.Sign(xspeed) * movedeceleration * ts;
						}
					}
					
					// Jump
					if ( jumpbuffer > 0.0f )
					{
						yspeed += jumpstrength;
						jumpbuffer = 0.0f;	// Reset jump buffer
						jumpheld = true;

						game.PlaySound("Jump");
					}

					animator.SetBool("InAir", false);
					animator.SetBool("IgnoreInAir", false);
				}
				// In Air
				else
				{
					// jumpheld variable is true as long as player is rising and holding the JUMP button.
					// When jumpheld is false, it stays false until set to true when jumping from the ground.
					jumpheld = jumpheld && (Input.GetButton("Jump") && yspeed > 0.0f);

					if (xlev > 0.0f) // Moving Right
					{
						xspeed = Mathf.Min(xspeed + moveaccelerationair * ts, movespeedmax);
					}
					else if (xlev < 0.0f)   // Moving Left
					{
						xspeed = Mathf.Max(xspeed - moveaccelerationair * ts, -movespeedmax);
					}

					animator.SetBool("InAir", true);
				}

				// Aiming up and down
				if (ylev > 0)
				{
					animator.SetBool("IgnoreInAir", true);
					aimingUp = true;
					aimingDown = false;
				}

				else if(ylev < 0)
				{
					animator.SetBool("IgnoreInAir", true);
					aimingDown = true;
					aimingUp = false;
				}

				else
				{
					aimingDown = false;
					aimingUp = false;
				}

				animator.SetBool("AimingUp", aimingUp);
				animator.SetBool("AimingDown", aimingDown);

				// Set gravity
				grav = jumpheld? gravityjump: gravity;
				break;
			}
			// -------------------------------------------------------------------
			// Defeat state when health is 0
			case(State.defeat): {
				grav = gravity * 0.2f;

				if (health > 0)
				{
					Start();
					animator.SetBool("Defeat", false);
				}
				break;
			}
		}

		// Update speeds
		if (!onground)
		{
			// Apply gravity
			yspeed = Mathf.Max(yspeed+grav*ts, -8.0f);
		}

		UpdateMovement();	// Move by xspeed, yspeed

		// Collision
		CollisionFlag collisionresult = EvaluateCollision(
			CollisionFlag.CHANGESPEED | CollisionFlag.DOUBLEX);
		if ( collisionresult.HasFlag(CollisionFlag.DOWN) )
		{
			yspeed = Mathf.Max(yspeed, 0.0f);	// When landing, keep upwards speed, if any
		}

		// Hitbox collision
		if (health > 0)
		{
			if (hurtboxcollider)
			{
				RaycastHit2D[] hitresults = new RaycastHit2D[8];
				CastHurtbox(hitresults);

				// Iterate through hits
				foreach (RaycastHit2D hit in hitresults)
				{
					if (hit.collider != null && hit.collider.enabled)
					{
						Entity e = GetEntityFromCollider(hit.collider);
						if (e && e.GetAttack() > 0)
						{
							// Do damage from hitbox
							DoDamage(e.GetAttack());
						}
					}
					
				}
			}
		}
		
		// Iframes
		if (iframes > 0.0f)
		{
			iframes = ApproachTS(iframes, 0.0f); // Decrement

			// Flicker
			if (iframes > 0.0f)
			{
				spriterenderer.enabled = Mathf.Repeat(iframes, 8.0f) < 4.0f;
			}
			else
			{
				spriterenderer.enabled = true;
			}
		}

		// Hide Player
		if (showplayer != game.GameFlagGet(_GameHeader.GameFlag.show_player))
		{
			showplayer = game.GameFlagGet(_GameHeader.GameFlag.show_player);
			spriterenderer.enabled = showplayer;
		}

		animator.speed = ts;
	}

	// Methods ===============================================================
	public PlayerData GetPlayerData() {return playerdata;}

	// Can check for animation stuff in OnShoot
	public void OnShoot(){
		//Shooting up from idle
		if(aimingUp && Mathf.Abs(xspeed) < 0.001 && Mathf.Abs(yspeed) < 0.001)
		{
			animator.SetBool("ShootingUp", true);
			ticks = 0;
			animator.Play("Base Layer.Kindle_shootup", 0, 0.0f);
		}

		//Shooting up while jumping 
		else if(aimingUp && Mathf.Abs(yspeed) > 0.001)
		{
			animator.SetBool("IgnoreInAir", true);
			animator.Play("Base Layer.Kindle_jump_shootup", 0, 0.0f);
		}

		//Shooting to the side from idle 
		else if(Mathf.Abs(xspeed) < 0.001 && Mathf.Abs(yspeed) < 0.001)
		{
			animator.SetBool("ShootingSide", true);
			ticks = 0;
			animator.Play("Base Layer.Kindle_shootside", 0, 0.0f);
		}

		//Shooting down (can only be done when jumping!)
		else if(aimingDown && Mathf.Abs(yspeed) > 0.001)
		{
			animator.SetBool("IgnoreInAir", true);
			animator.Play("Base Layer.Kindle_jump_shootdown", 0, 0.0f);
		}
	}

	// Called when picking up a heart, taking damage, etc.
	public override int ChangeHealth(int value)
	{
		// Losing health
		if (value < 0 && iframes == 0.0f)
		{
			int healthdiff = base.ChangeHealth(value);
			// Subtract energy when health is lost
			if (healthdiff < 0)
			{
				playerdata.AddEnergy(healthdiff);
				playerdata.SetHealth(health, healthmax); // Update HUD
			}
			return healthdiff;
		}
		// Gaining health
		else if (value > 0)
		{
			int healthdiff = base.ChangeHealth(value);

			// Flash when health is gained
			if (healthdiff > 0)
			{
				playerdata.HealthFlashMeter();
				playerdata.SetHealth(health, healthmax); // Update HUD
			}
		}

		return 0;
	}

	// Called after changing health value
	protected override void OnHealthChange(int diff)
	{
		if (diff < 0)
		{
			iframes = iframestime;
			yspeed = 5.0f;
			jumpheld = true;
			onground = false;
		}
	}

	// Called when resulting health change results in a health of 0
	protected override bool OnDefeat()
	{
		state = (int)State.defeat;
		animator.SetBool("Defeat", true);
		Instantiate(gameover_prefab).transform.parent = null;
		iframes = 0;

		yspeed = 6.0f;
		xspeed = -hsign * 2.0f;

		state = (int)State.defeat;
		return false;
	}

	// Utility ================================================================

	public float GetHSign() {return hsign;}
	public float GetVSign() {return vsign;}
	public bool GetOnGround() {return onground;}
}
