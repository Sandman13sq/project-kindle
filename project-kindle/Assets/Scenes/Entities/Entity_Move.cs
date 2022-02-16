using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Move : MonoBehaviour
{
    // Variables =========================================

    public float x, y;
    public float xspeed, yspeed;

    private Rigidbody2D rigBod;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60; // Temporary. Will remove later

        x = transform.position.x;
        y = transform.position.y;

        rigBod = GetComponent<Rigidbody2D>();
        rigBod.drag = 0.0f;
        //rigBod.useGravity = false;
    }

    void OnCollisionEnter(Collision collisioninfo)
	{
        Debug.Log(collisioninfo.collider.name);
        yspeed = 0.0f;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        //xspeed = rigBod.velocity.x;
        //yspeed = rigBod.velocity.y;

        float moveacc = 3.0f;   // Acceleration
        float movedec = 5.0f;   // Deceleration
        float movespeedmax = 40.0f;
        float gravity = -1.0f;

        // Grab input values
        float xlev = Input.GetAxisRaw("Horizontal");
        float ylev = Input.GetAxisRaw("Vertical");


        // Ground Movement
        /*
            When player is moving in the same direction as the input, use acceleration and approach movespeed.
            When player is moving in the opposing direction as the input, use deceleration and approach movespeed.
            When no input is held, use deceleration and approach 0.
        */
        if ( xlev > 0.0f ) // Moving Right
        {
            if (xspeed < 0.0f)  // Moving against direction, use deceleration
			{
                xspeed = Mathf.Min(xspeed + movedec, movespeedmax);
            }
            else    // Moving with direction, use acceleration
			{
                xspeed = Mathf.Min(xspeed + moveacc, movespeedmax);
            }
            
		}
        else if (xlev < 0.0f)   // Moving Left
        {
            if (xspeed > 0.0f)  // Moving against direction, use deceleration
            {
                xspeed = Mathf.Max(xspeed - movedec, -movespeedmax);
            }
            else    // Moving with direction, use acceleration
            {
                xspeed = Mathf.Max(xspeed - moveacc, -movespeedmax);
            }
        }
        else    // No input held
		{
            if (Mathf.Abs(xspeed) <= movedec) // Clamp to 0 when speed is small enough
            {
                xspeed = 0.0f;
            }
            else    // Approach 0 with deceleration
            {
                xspeed -= Mathf.Sign(xspeed) * movedec;
            }
		}

        //yspeed += gravity;

        // Update speeds
        //x += xspeed;
        //y += yspeed;    // No values yet

        //rigBod.MovePosition( new Vector3(x, y, 0.0f) );
        //rigBod.AddForce(Vector3.right * 1.0f, ForceMode.Acceleration);
        rigBod.AddForce(new Vector3(xlev * moveacc, gravity, 0.0f), ForceMode2D.Impulse);

        // Update object position
        //transform.position += new Vector3(xspeed, yspeed, 0.0f);

        xspeed = rigBod.velocity.x;
        yspeed = rigBod.velocity.y;
    }


}
