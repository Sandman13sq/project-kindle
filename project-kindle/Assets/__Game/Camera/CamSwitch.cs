using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    private Animator animator;//Get the animator so that we can get it's state

    //Grab data from kindle to see which direction she is facing
    Entity_Move_Manual kindle;
    [SerializeField] GameObject playerEntity;

    //Keep track of what direction we're lookin' in
    public float lookingAt = 4;//4 = right, 3 = left, 2 = up, 1 = down

    // Awake is called as soon as the object shows up in the scene, called before start()
    void Awake(){
        kindle = playerEntity.GetComponent<Entity_Move_Manual>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SetLookAt();
        ChangeCam();
    }

    void SetLookAt(){
        float horizontalAim = kindle.GetHSign();
        float verticalAim = kindle.GetVSign();

        if(horizontalAim == 1){
            lookingAt = 4;
        }
        else if(horizontalAim == -1){
            lookingAt = 3;
        }

        if(verticalAim == 1){
            lookingAt = 2;
        }

        else if (verticalAim == -1){
            lookingAt = 1;
        }
    }

    void ChangeCam(){
        if (lookingAt == 4){
            animator.Play("RightCam");
        }

        else if (lookingAt == 3){
            animator.Play("LeftCam");
        }

        else if (lookingAt == 2){
            animator.Play("UpCam");
        }

        else if (lookingAt == 1){
            animator.Play("DownCam");
        }
    }
}
