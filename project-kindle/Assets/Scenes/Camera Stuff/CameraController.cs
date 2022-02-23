using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("right")){
            animator.Play("Right Cam");
        }
        else if(Input.GetKeyDown("left")){
            animator.Play("Left Cam");
        }
        else if(Input.GetKeyDown("up")){
            animator.Play("Up Cam");
        }
        else if(Input.GetKeyDown("down")){
            animator.Play("Down Cam");
        }
    }
}
