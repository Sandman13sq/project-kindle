using UnityEngine;
using Cinemachine;

public class CamPriority : MasterObject
{
    //Grab data from kindle to see which direction she is facing
    //Keep track of what direction we're lookin' in
    public float lookingAt = 4;//4 = right, 3 = left, 2 = up, 1 = down

    //Keep track of Cams
    [SerializeField] private CinemachineVirtualCamera RightCam;
    [SerializeField] private CinemachineVirtualCamera LeftCam;
    [SerializeField] private CinemachineVirtualCamera UpCam;
    [SerializeField] private CinemachineVirtualCamera DownCam;

    // Awake is called as soon as the object shows up in the scene, called before start()
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetLookAt();
        ChangeCam();
    }

    void SetLookAt(){
        var p = game.GetPlayer();

        if (p == null) {return;}

        float horizontalAim = p.GetHSign();
        float verticalAim = p.GetVSign();

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
            RightCam.Priority = 10;
            LeftCam.Priority = 0;
            UpCam.Priority = 0;
            DownCam.Priority = 0;
        }

        else if (lookingAt == 3){
            RightCam.Priority = 0;
            LeftCam.Priority = 10;
            UpCam.Priority = 0;
            DownCam.Priority = 0;
        }

        else if (lookingAt == 2){
            RightCam.Priority = 0;
            LeftCam.Priority = 0;
            UpCam.Priority = 10;
            DownCam.Priority = 0;
        }

        else if (lookingAt == 1){
            RightCam.Priority = 0;
            LeftCam.Priority = 0;
            UpCam.Priority = 0;
            DownCam.Priority = 10;
        }
    }
}
