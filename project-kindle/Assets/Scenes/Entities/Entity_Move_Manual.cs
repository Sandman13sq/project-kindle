using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Move_Manual : MonoBehaviour
{
	// Variables =========================================

	public float x, y;
	public float xspeed, yspeed;

	// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60; // Temporary. Will remove later

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
		float movespeedmax = 4.0f;

		// Grab input values
		float xlev = Input.GetAxisRaw("Horizontal");
		float ylev = Input.GetAxisRaw("Vertical");

		xlev = (Mathf.Abs(xlev) >= 1.0f) ? Mathf.Sign(xlev) : 0.0f; // Make value -1, 0, or 1
		ylev = (Mathf.Abs(ylev) >= 1.0f) ? Mathf.Sign(ylev) : 0.0f; // Make value -1, 0, or 1

		// Ground Movement
		/*
			When player is moving in the same direction as the input, use acceleration and approach movespeed.
			When player is moving in the opposing direction as the input, use deceleration and approach movespeed.
			When no input is held, use deceleration and approach 0.
		*/
		if (xlev > 0.0f) // Moving Right
		{
			if (xspeed < 0.0f)  // Moving against direction, use deceleration
			{
				xspeed = Mathf.Min(xspeed + movedeceleration, movespeedmax);
			}
			else    // Moving with direction, use acceleration
			{
				xspeed = Mathf.Min(xspeed + moveacceleration, movespeedmax);
			}

		}
		else if (xlev < 0.0f)   // Moving Left
		{
			if (xspeed > 0.0f)  // Moving against direction, use deceleration
			{
				xspeed = Mathf.Max(xspeed - movedeceleration, -movespeedmax);
			}
			else    // Moving with direction, use acceleration
			{
				xspeed = Mathf.Max(xspeed - moveacceleration, -movespeedmax);
			}
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

		// Update speeds
		x += xspeed;
		y += yspeed;    // No values yet

		// Update object position
		transform.position = new Vector3(x, y, 0.0f);
	}


}
