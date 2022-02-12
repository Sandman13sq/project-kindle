using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControls : MonoBehaviour
{
    private float moveSpeed = 10;
    private CharacterController controller;

    private void Start() {
        controller = gameObject.GetComponent<CharacterController>();
        Debug.Log("Tatu wuz here!");
    }

    // Update is called once per frame
    void Update()
    {
        movementHandler();
    }

    void movementHandler() {
        float upDown = Input.GetAxis("Vertical"); //Up = 1, Down = -1
        float leftRite = Input.GetAxis("Horizontal"); //left = -1, right = 1

        //Debug.Log("upDown is: " + upDown);
        //Debug.Log("leftRite is: " + leftRite);

        Vector3 moveVect = new Vector3(leftRite, upDown, 0.0f) * Time.deltaTime * moveSpeed;
        controller.Move(moveVect);
    }
}
