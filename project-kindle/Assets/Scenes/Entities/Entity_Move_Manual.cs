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

	// Common ===============================================================
	
	// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60; // Temporary. Will remove later

		playerdata.SetHealth(health);
	}

	// Update is called once per frame
	void Update()
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

		// Flip sprite if moving left
        if (xlev != 0.0)
        {
			rightsign = (xlev > 0.0f)? 1.0f: -1.0f;
            spriterenderer.flipX = xlev < 0.0f;
        }

		// Jump buffer
        if (jumpbuffer >= 0.0f)
        {
            jumpbuffer -= 1.0f;
        }

		if (Input.GetKeyDown("z"))
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
			jumpheld = jumpheld && Input.GetKey("z") && yspeed > 0.0f;

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

		// temp schüt
		if (Input.GetKeyDown("x"))
		{
			float xaim, yaim;

			if (ylev != 0.0f)
			{
				xaim = 0.0f;
				yaim = ylev;
			}
			else
			{
				xaim = rightsign;
				yaim = 0.0f;
			}

			WeaponProjectile proj = Instantiate(projectile).GetComponent<WeaponProjectile>();
			proj.transform.position = transform.position + new Vector3(xaim*40.0f, yaim*48.0f, 0.0f);
			proj.SetDirectionRad(Mathf.Atan2(yaim, xaim), rightsign);
			recentshot = 0;
		}

		recentshot += 1;

		// Temporary Spriting
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
	}

	// Methods ===============================================================

	public PlayerData GetPlayerData() {return playerdata;}

	public override int ChangeHealth(int value)
	{
		int healthdiff = base.ChangeHealth(value);
		playerdata.SetHealth(health);

		return healthdiff;
	}
}
