using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //constants for movement
    private float speedCap = 8.0f;
    private float jumpHeight = 8.0f;
    private float moveAccel = 0.3f;
    private float jumpAccel = 1.5f;

    //variable to hold the rigidbody component in case we need to manipulate it
    private Rigidbody2D rigBod;

    //variable to hold the current movement speed
    private float moveSpeed = 0.0f;
     private float jumpSpeed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        rigBod = gameObject.GetComponent<Rigidbody2D>();
        Debug.Log("Tatu wuz here!");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //calls movementHandler every frame
        movementHandler();
    }

    //Handles movement by check if left or right are being pressed or if the jump key is being held down
    void movementHandler(){
        Vector3 moveVect = new Vector3(moveSpeed, 0.0f, 0.0f) * Time.deltaTime;
        
        //*****MOVING RIGHT********
        //when right arrow is held down, move in the positive x direction
        if(Input.GetKey("right")){
            //we only accelerate until we hit the movement speed cap
            if(Mathf.Abs(moveSpeed) < speedCap){
                moveSpeed += moveAccel;
            }
            transform.position += moveVect;
        }

        //******MOVING LEFT********
        //when left arrow is held down, move in the negative x direction
        if(Input.GetKey("left")){
            //we only accelerate until we hit the movement speed cap
            if(Mathf.Abs(moveSpeed) < speedCap){
                moveSpeed -= moveAccel;
            }
            transform.position += moveVect;
        }

        //slow down if we are moving still and have let go of a movement key
        if (!Input.GetKey("right") && !Input.GetKey("left")){
            //if the velocity of out sprite is going too far right or left, move it in the opposite direction
            //moving right still
            if(moveSpeed > 0){
                transform.position += moveVect;
                moveSpeed -= moveAccel;
            }

            //moving left still
            if(moveSpeed < 0){
                transform.position += moveVect;
                moveSpeed += moveAccel;
            }

            //rounding issues occur with this implementation, if we are moving less than 0.1, then set it to 0
            if(Mathf.Abs(moveSpeed) < 0.1){
                moveSpeed = 0.0f;
            }
        }

        //****JUMPING*****
        Debug.Log("Jumpspeed: " + jumpSpeed);
        //Check if Z is being pressed, if so, jump!
        if(Input.GetKeyDown("z") && Mathf.Abs(jumpSpeed) <= 0.01){
            jumpSpeed = jumpHeight;
            Vector3 jumpVect = new Vector3(0.0f, jumpSpeed, 0.0f) * Time.deltaTime * jumpHeight;
            transform.position += jumpVect;
            jumpSpeed -= jumpAccel;
        }
    }
}
