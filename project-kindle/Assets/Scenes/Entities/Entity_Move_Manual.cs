using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Move_Manual : MonoBehaviour
{
	// Variables =========================================

	public float x, y;
	public float xspeed, yspeed;
	private const float floory = 136.0f;

	public bool onground;
	public bool inair;
	public bool jumpheld;
	public float jumpbuffer;
    private float jumpbuffertime = 7.0f; // Max number of frames ahead of time where a jump press will still be read

	public Collider2D collider;
	private SpriteRenderer spriterdr;

	// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60; // Temporary. Will remove later

		spriterdr = GetComponent<SpriteRenderer>();
		
		x = transform.position.x;
		y = transform.position.y;
		xspeed = 0.0f;
		yspeed = 0.0f;
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
            spriterdr.flipX = xlev < 0.0f;
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
		x += xspeed;
		y += yspeed;    // No values yet

		// Ground Check
		float ydiff = Mathf.Abs(spriterdr.sprite.bounds.min.y-spriterdr.sprite.bounds.center.y);
		
		RaycastHit2D groundcollision = Physics2D.Raycast(
            new Vector2(x, y), 
            new Vector2(0.0f, -1.0f), 
            ydiff+1.0f
            );
		
		if (groundcollision.collider != null)
		{
			//Debug.Log(groundcollision.collider.name);
			y = groundcollision.point.y+ydiff;
			yspeed = Mathf.Max(yspeed, 0.0f);
			onground = true;
		}
		else
		{
			onground = false;
		}

		inair = !onground;

		// Update object position
		transform.position = new Vector3(x, y, 0.0f);
	}

}
