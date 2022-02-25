using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControls : MonoBehaviour
{
    private float moveSpeed = 8;
    private float jumpBump = 4.0f;
    
    private Rigidbody2D rigBod;

    private void Start() {
        rigBod = gameObject.GetComponent<Rigidbody2D>();
        Debug.Log("Tatu wuz here!");
    }

    // Update is called once per frame
    void Update()
    {
        movementHandler();
    }

    void movementHandler() {
        float leftRite = Input.GetAxis("Horizontal"); //left = -1, right = 1

        Vector3 moveVect = new Vector3(leftRite, 0.0f, 0.0f) * Time.deltaTime * moveSpeed;
        Vector2 jumpStr = new Vector2(0, jumpBump);
        transform.position += moveVect;

        if(Input.GetKeyDown("z") && Mathf.Abs(rigBod.velocity.y) < 0.001f){
            rigBod.AddForce(jumpStr, ForceMode2D.Impulse);
        }
    }
}
